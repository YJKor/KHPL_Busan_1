using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// 화살 발사 및 비행을 담당하는 컴포넌트
/// XR Pull Interactable과 연동되어 활쏘기 시스템의 핵심 역할을 합니다.
/// ArrowImpactHandler와 함께 사용되어 완전한 활쏘기 시스템을 구성합니다.
/// </summary>
public class ArrowLauncher : MonoBehaviour
{
    [Header("Launch Settings")]
    [Tooltip("화살 발사 속도 배수")]
    [SerializeField] private float _speed = 15f;

    [Tooltip("화살 회전력")]
    [SerializeField] private float _spinForce = 10f;

    [Tooltip("최대 발사 거리")]
    [SerializeField] private float _maxDistance = 50f;

    [Header("Visual Effects")]
    [Tooltip("화살 비행 중 표시할 트레일 시스템")]
    [SerializeField] private GameObject _trailSystem;

    [Tooltip("화살 발사 시 이펙트")]
    [SerializeField] private GameObject _launchEffectPrefab;

    [Header("Audio")]
    [Tooltip("화살 발사 사운드")]
    [SerializeField] private AudioClip _launchSound;

    [Header("Physics")]
    [Tooltip("화살 발사 후 물리 활성화")]
    [SerializeField] private bool _enablePhysicsOnLaunch = true;

    [Tooltip("중력 영향 받기")]
    [SerializeField] private bool _useGravity = true;

    // 내부 변수
    private Rigidbody _rigidBody;
    private bool _inAir = false;
    private MonoBehaviour _pullInteractable; // XRPullInteractable 대신 MonoBehaviour 사용
    private AudioSource _audioSource;
    private ArrowImpactHandler _impactHandler;
    private Vector3 _launchPosition;
    private float _launchTime;

    // 이벤트
    public System.Action OnArrowLaunched;
    public System.Action OnArrowStopped;
    public System.Action<float> OnPullStrengthChanged; // 당김 강도 이벤트 추가

    private void Awake()
    {
        InitializeComponents();
        SetPhysics(false);
    }

    /// <summary>
    /// 필요한 컴포넌트들을 초기화합니다.
    /// </summary>
    private void InitializeComponents()
    {
        // Rigidbody 컴포넌트 찾기
        _rigidBody = GetComponent<Rigidbody>();
        if (_rigidBody == null)
        {
            Debug.LogError($"Rigidbody component not found on Arrow {gameObject.name}");
        }

        // AudioSource 컴포넌트 찾기 또는 추가
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ArrowImpactHandler 컴포넌트 찾기
        _impactHandler = GetComponent<ArrowImpactHandler>();
    }

    /// <summary>
    /// Pull Interactable과 연동을 초기화합니다.
    /// </summary>
    /// <param name="pullInteractable">연동할 Pull Interactable</param>
    public void Initialize(MonoBehaviour pullInteractable)
    {
        _pullInteractable = pullInteractable;
        
        // 리플렉션을 사용하여 이벤트 연결
        var eventInfo = pullInteractable.GetType().GetEvent("PullActionReleased");
        if (eventInfo != null)
        {
            var delegateType = eventInfo.EventHandlerType;
            var methodInfo = GetType().GetMethod("Release", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (methodInfo != null)
            {
                var handler = System.Delegate.CreateDelegate(delegateType, this, methodInfo);
                eventInfo.AddEventHandler(pullInteractable, handler);
            }
        }
    }

    /// <summary>
    /// 수동으로 화살을 발사합니다.
    /// </summary>
    /// <param name="pullStrength">당김 강도 (0-1)</param>
    public void LaunchArrow(float pullStrength)
    {
        if (_inAir) return; // 이미 비행 중이면 발사하지 않음

        // 화살을 부모에서 분리
        gameObject.transform.parent = null;
        
        // 발사 상태 설정
        _inAir = true;
        _launchPosition = transform.position;
        _launchTime = Time.time;

        // 물리 활성화
        if (_enablePhysicsOnLaunch)
        {
            SetPhysics(true);
        }

        // 발사 힘 계산 및 적용
        Vector3 force = transform.forward * pullStrength * _speed;
        if (_rigidBody != null)
        {
            _rigidBody.AddForce(force, ForceMode.Impulse);

            // 회전력 추가
            if (_spinForce > 0)
            {
                _rigidBody.AddTorque(transform.right * _spinForce, ForceMode.Impulse);
            }
        }

        // 발사 이펙트 생성
        CreateLaunchEffect();

        // 발사 사운드 재생
        PlayLaunchSound();

        // 트레일 시스템 활성화
        if (_trailSystem != null)
        {
            _trailSystem.SetActive(true);
        }

        // 이벤트 호출
        OnArrowLaunched?.Invoke();
        OnPullStrengthChanged?.Invoke(pullStrength);

        // 속도에 따른 회전 코루틴 시작
        StartCoroutine(RotateWithVelocity());
    }

    /// <summary>
    /// 화살을 발사합니다. (이벤트 핸들러용)
    /// </summary>
    /// <param name="value">당김 강도 (0-1)</param>
    private void Release(float value)
    {
        LaunchArrow(value);
    }

    private void OnDestroy()
    {
        // 이벤트 리스너 제거
        if (_pullInteractable != null)
        {
            var eventInfo = _pullInteractable.GetType().GetEvent("PullActionReleased");
            if (eventInfo != null)
            {
                var methodInfo = GetType().GetMethod("Release", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (methodInfo != null)
                {
                    var delegateType = eventInfo.EventHandlerType;
                    var handler = System.Delegate.CreateDelegate(delegateType, this, methodInfo);
                    eventInfo.RemoveEventHandler(_pullInteractable, handler);
                }
            }
        }
    }

    /// <summary>
    /// 발사 이펙트를 생성합니다.
    /// </summary>
    private void CreateLaunchEffect()
    {
        if (_launchEffectPrefab != null)
        {
            GameObject effect = Instantiate(_launchEffectPrefab, transform.position, transform.rotation);
            Destroy(effect, 2f);
        }
    }

    /// <summary>
    /// 발사 사운드를 재생합니다.
    /// </summary>
    private void PlayLaunchSound()
    {
        if (_launchSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_launchSound);
        }
    }

    /// <summary>
    /// 화살이 속도 방향으로 회전하도록 하는 코루틴
    /// </summary>
    private IEnumerator RotateWithVelocity()
    {
        yield return new WaitForFixedUpdate();
        
        while (_inAir)
        {
            if (_rigidBody != null && _rigidBody.velocity.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(_rigidBody.velocity, transform.up);
            }
            yield return null;
        }
    }

    /// <summary>
    /// 화살 비행을 중지합니다.
    /// </summary>
    public void StopFlight()
    {
        _inAir = false;
        SetPhysics(false);
        
        if (_trailSystem != null)
        {
            _trailSystem.SetActive(false);
        }

        OnArrowStopped?.Invoke();
    }

    /// <summary>
    /// 물리 효과를 설정합니다.
    /// </summary>
    /// <param name="enabled">물리 활성화 여부</param>
    private void SetPhysics(bool enabled)
    {
        if (_rigidBody != null)
        {
            _rigidBody.isKinematic = !enabled;
            _rigidBody.useGravity = enabled && _useGravity;
        }
    }

    /// <summary>
    /// 화살이 비행 중인지 확인합니다.
    /// </summary>
    /// <returns>비행 중 여부</returns>
    public bool IsInAir()
    {
        return _inAir;
    }

    /// <summary>
    /// 화살이 최대 거리를 초과했는지 확인합니다.
    /// </summary>
    /// <returns>최대 거리 초과 여부</returns>
    public bool HasExceededMaxDistance()
    {
        if (!_inAir) return false;
        
        float distance = Vector3.Distance(_launchPosition, transform.position);
        return distance > _maxDistance;
    }

    /// <summary>
    /// 화살의 현재 속도를 반환합니다.
    /// </summary>
    /// <returns>현재 속도</returns>
    public Vector3 GetCurrentVelocity()
    {
        return _rigidBody != null ? _rigidBody.velocity : Vector3.zero;
    }

    /// <summary>
    /// 화살의 발사 시간을 반환합니다.
    /// </summary>
    /// <returns>발사 시간</returns>
    public float GetLaunchTime()
    {
        return _launchTime;
    }

    /// <summary>
    /// 화살의 발사 위치를 반환합니다.
    /// </summary>
    /// <returns>발사 위치</returns>
    public Vector3 GetLaunchPosition()
    {
        return _launchPosition;
    }

    /// <summary>
    /// 화살의 비행 시간을 반환합니다.
    /// </summary>
    /// <returns>비행 시간</returns>
    public float GetFlightTime()
    {
        return _inAir ? Time.time - _launchTime : 0f;
    }

    /// <summary>
    /// 화살의 발사 속도를 설정합니다.
    /// </summary>
    /// <param name="speed">새로운 발사 속도</param>
    public void SetLaunchSpeed(float speed)
    {
        _speed = speed;
    }

    /// <summary>
    /// 화살의 회전력을 설정합니다.
    /// </summary>
    /// <param name="spinForce">새로운 회전력</param>
    public void SetSpinForce(float spinForce)
    {
        _spinForce = spinForce;
    }

    /// <summary>
    /// 화살의 최대 거리를 설정합니다.
    /// </summary>
    /// <param name="maxDistance">새로운 최대 거리</param>
    public void SetMaxDistance(float maxDistance)
    {
        _maxDistance = maxDistance;
    }

    void Update()
    {
        // 최대 거리 초과 시 화살 파괴
        if (_inAir && HasExceededMaxDistance())
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 디버그 정보를 출력합니다.
    /// </summary>
    [ContextMenu("Debug Arrow Info")]
    public void DebugArrowInfo()
    {
        Debug.Log($"=== 화살 디버그 정보 ===");
        Debug.Log($"화살 이름: {gameObject.name}");
        Debug.Log($"비행 중: {_inAir}");
        Debug.Log($"Rigidbody: {(_rigidBody != null ? "있음" : "없음")}");
        Debug.Log($"AudioSource: {(_audioSource != null ? "있음" : "없음")}");
        Debug.Log($"ArrowImpactHandler: {(_impactHandler != null ? "있음" : "없음")}");
        Debug.Log($"발사 속도: {_speed}");
        Debug.Log($"회전력: {_spinForce}");
        Debug.Log($"최대 거리: {_maxDistance}");
        Debug.Log($"현재 속도: {GetCurrentVelocity()}");
        Debug.Log($"비행 시간: {GetFlightTime()}");
        Debug.Log("========================");
    }
}