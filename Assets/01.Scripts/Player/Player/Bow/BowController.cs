using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

/// <summary>
/// VR 활쏘기 컨트롤러 - XR Interaction Toolkit 기반
/// 활 시위 조작, 화살 발사, 물리 시뮬레이션을 담당
/// ArrowLauncher와 ArrowImpactHandler와 완벽하게 연동됩니다.
/// </summary>
public class BowController : MonoBehaviour
{
    [Header("String & Nocking")]
    [Tooltip("활 시위를 표시할 라인 렌더러")]
    [SerializeField] private LineRenderer bowStringRenderer;
    
    [Tooltip("시위 시작점")]
    [SerializeField] private Transform stringStartPoint;
    
    [Tooltip("시위 끝점")]
    [SerializeField] private Transform stringEndPoint;
    
    [Tooltip("화살을 끼우는 소켓")]
    [SerializeField] private XRSocketInteractor nockSocket;

    [Header("Arrow System")]
    [Tooltip("화살 프리팹")]
    [SerializeField] private GameObject arrowPrefab;
    
    [Tooltip("화살 생성 위치")]
    [SerializeField] private Transform arrowSpawnPoint;
    
    [Tooltip("자동 화살 생성 간격 (초)")]
    [SerializeField] private float arrowSpawnInterval = 2f;
    
    [Tooltip("최대 보유 화살 수")]
    [SerializeField] private int maxArrows = 10;

    [Header("Shooting")]
    [Tooltip("발사 힘 배수")]
    [SerializeField] private float shootingForceMultiplier = 25f;
    
    [Tooltip("최대 당김 거리")]
    [SerializeField] private float maxPullDistance = 0.6f;
    
    [Tooltip("시위 장력 배수")]
    [SerializeField] private float stringTension = 1.2f;

    [Header("Visual & Audio")]
    [Tooltip("시위 당김 사운드")]
    [SerializeField] private AudioClip pullSound;
    
    [Tooltip("화살 발사 사운드")]
    [SerializeField] private AudioClip releaseSound;
    
    [Tooltip("화살 장착 사운드")]
    [SerializeField] private AudioClip nockSound;
    
    [Tooltip("시위 색상")]
    [SerializeField] private Color stringColor = Color.white;
    
    [Tooltip("시위 두께")]
    [SerializeField] private float stringWidth = 0.005f;

    [Header("Debug")]
    [Tooltip("디버그 로그 활성화")]
    [SerializeField] private bool enableDebugLogs = true;

    // 내부 변수
    private IXRSelectInteractable nockedArrow = null;
    private bool isArrowNocked = false;
    private bool isStringPulled = false;
    private Transform pullingHand = null;
    private float currentPullDistance = 0f;
    private Vector3 originalStringPosition;
    private AudioSource audioSource;
    private int currentArrowCount = 0;
    private Coroutine arrowSpawnCoroutine;
    private XRPullInteractable stringPullInteractable;

    // 이벤트
    public System.Action<float> OnPullStrengthChanged;
    public System.Action OnArrowReleased;
    public System.Action<int> OnArrowCountChanged;

    private void Awake()
    {
        InitializeBow();
    }

    private void OnDestroy()
    {
        // 이벤트 리스너 제거
        if (nockSocket != null)
        {
            nockSocket.selectEntered.RemoveListener(OnArrowNocked);
            nockSocket.selectExited.RemoveListener(OnArrowRemoved);
        }
        
        if (stringPullInteractable != null)
        {
            stringPullInteractable.PullActionReleased -= OnStringReleased;
        }
    }

    /// <summary>
    /// 활 초기화
    /// </summary>
    private void InitializeBow()
    {
        // AudioSource 컴포넌트 찾기 또는 추가
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 시위 초기화
        SetupBowString();

        // 이벤트 리스너 등록
        if (nockSocket != null)
        {
            nockSocket.selectEntered.AddListener(OnArrowNocked);
            nockSocket.selectExited.AddListener(OnArrowRemoved);
        }

        // 시위 당김 감지 설정
        SetupStringPullDetection();

        // 자동 화살 생성 시작
        StartArrowSpawning();

        if (enableDebugLogs)
            Debug.Log("향상된 활 컨트롤러가 초기화되었습니다.");
    }

    /// <summary>
    /// 활 시위 설정
    /// </summary>
    private void SetupBowString()
    {
        if (bowStringRenderer == null)
        {
            bowStringRenderer = gameObject.AddComponent<LineRenderer>();
        }

        bowStringRenderer.material = new Material(Shader.Find("Sprites/Default"));
        bowStringRenderer.startColor = stringColor;
        bowStringRenderer.endColor = stringColor;
        bowStringRenderer.startWidth = stringWidth;
        bowStringRenderer.endWidth = stringWidth;
        bowStringRenderer.positionCount = 2;
        bowStringRenderer.useWorldSpace = true;

        // 시위 기본 위치 설정
        if (stringStartPoint != null && stringEndPoint != null)
        {
            originalStringPosition = (stringStartPoint.position + stringEndPoint.position) * 0.5f;
            ResetBowString();
        }
    }

    /// <summary>
    /// 시위 당김 감지 설정
    /// </summary>
    private void SetupStringPullDetection()
    {
        // XRPullInteractable 컴포넌트 찾기 또는 추가
        stringPullInteractable = GetComponent<XRPullInteractable>();
        if (stringPullInteractable == null)
        {
            stringPullInteractable = gameObject.AddComponent<XRPullInteractable>();
        }

        // 시위 당김 이벤트 등록
        stringPullInteractable.PullActionReleased += OnStringReleased;
    }

    /// <summary>
    /// 자동 화살 생성 시작
    /// </summary>
    private void StartArrowSpawning()
    {
        if (arrowSpawnCoroutine != null)
        {
            StopCoroutine(arrowSpawnCoroutine);
        }
        arrowSpawnCoroutine = StartCoroutine(AutoArrowSpawn());
    }

    /// <summary>
    /// 자동 화살 생성 코루틴
    /// </summary>
    private IEnumerator AutoArrowSpawn()
    {
        while (currentArrowCount < maxArrows)
        {
            yield return new WaitForSeconds(arrowSpawnInterval);
            if (currentArrowCount < maxArrows)
            {
                SpawnArrowInHand();
            }
        }
    }

    /// <summary>
    /// 손에 화살 생성
    /// </summary>
    private void SpawnArrowInHand()
    {
        if (arrowPrefab != null && arrowSpawnPoint != null)
        {
            GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
            SetupArrowComponents(newArrow);
            currentArrowCount++;
            OnArrowCountChanged?.Invoke(currentArrowCount);

            if (enableDebugLogs)
                Debug.Log($"화살이 생성되었습니다. 현재 화살 수: {currentArrowCount}");
        }
    }

    /// <summary>
    /// 화살 컴포넌트 설정
    /// </summary>
    /// <param name="arrow">설정할 화살 오브젝트</param>
    private void SetupArrowComponents(GameObject arrow)
    {
        // ArrowLauncher 컴포넌트 설정
        ArrowLauncher launcher = arrow.GetComponent<ArrowLauncher>();
        if (launcher != null)
        {
            launcher.Initialize(stringPullInteractable);
            
            // 이벤트 연결
            launcher.OnArrowLaunched += () => {
                currentArrowCount--;
                OnArrowCountChanged?.Invoke(currentArrowCount);
                OnArrowReleased?.Invoke();
                
                if (enableDebugLogs)
                    Debug.Log($"화살이 발사되었습니다. 남은 화살 수: {currentArrowCount}");
            };
        }

        // ArrowImpactHandler 컴포넌트 확인
        ArrowImpactHandler impactHandler = arrow.GetComponent<ArrowImpactHandler>();
        if (impactHandler == null)
        {
            impactHandler = arrow.AddComponent<ArrowImpactHandler>();
        }

        // Rigidbody 설정
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Collider 설정
        Collider col = arrow.GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = false;
        }
    }

    /// <summary>
    /// 시위 당김 해제 이벤트 처리
    /// </summary>
    /// <param name="value">당김 강도 (0-1)</param>
    private void OnStringReleased(float value)
    {
        if (isArrowNocked && nockedArrow != null)
        {
            // 화살 발사
            FireArrow(value);
            
            // 시위 리셋
            ResetBowString();
            
            // 발사 사운드 재생
            if (releaseSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(releaseSound);
            }

            if (enableDebugLogs)
                Debug.Log($"화살 발사! 당김 강도: {value}");
        }
    }

    // Update 함수
    void Update()
    {
        if (isArrowNocked && nockedArrow != null)
        {
            // 디버그 로그 추가
            if (enableDebugLogs)
                Debug.Log("Update: 화살이 장착되어 있습니다. 손 위치를 찾습니다.");

            // 화살이 장착되어 있다면, 화살을 잡고 있는 손의 위치를 찾아서 시위를 업데이트합니다.
            IXRSelectInteractor hand = nockedArrow.firstInteractorSelecting;
            if (hand != null)
            {
                // 손 위치로 시위를 업데이트
                UpdateBowString(hand.transform.position);

                if (enableDebugLogs)
                    Debug.Log($"손 위치: {hand.transform.position}, 시위 업데이트됨");
            }
            else
            {
                if (enableDebugLogs)
                    Debug.LogWarning("손을 찾을 수 없습니다!");
            }
        }
        else
        {
            // 화살이 장착되지 않았다면 기본 위치로 되돌립니다.
            ResetBowString();
        }
    }

    // 화살을 소켓에 끼우고 있을 때 호출되는 함수
    private void OnArrowNocked(SelectEnterEventArgs args)
    {
        // 디버그 로그 추가
        if (enableDebugLogs)
            Debug.Log("화살을 소켓에 끼우고 있는 것이 감지되었습니다! (OnArrowNocked 호출)");

        nockedArrow = args.interactableObject;
        isArrowNocked = true;

        // 화살을 소켓에서 제거할 때(발사되거나 그냥 제거될 때) Shoot 함수를 호출하도록 이벤트를 설정
        nockedArrow.selectExited.AddListener(Shoot);

        //if (enableDebugLogs)
        //    Debug.Log($"화살 장착 완료: {nockedArrow.name}");
    }

    // 화살을 소켓에서 제거할 때(발사되거나 그냥 제거될 때) 호출
    private void OnArrowRemoved(SelectExitEventArgs args)
    {
        // 이벤트 리스너 제거
        if (args.interactableObject == nockedArrow)
        {
            nockedArrow.selectExited.RemoveListener(Shoot);
            ResetBowString();
            nockedArrow = null;
            isArrowNocked = false;

            if (enableDebugLogs)
                Debug.Log("화살이 제거되었습니다.");
        }
    }

    // 화살 발사 함수
    private void Shoot(SelectExitEventArgs args)
    {
        if (enableDebugLogs)
            Debug.Log("화살 발사! (Shoot 함수 호출)");

        // args.interactorObject는 화살을 잡고 있는 손(컨트롤러)
        // 당김 거리를 계산합니다 (손과 소켓 사이의 거리)
        float pullDistance = Vector3.Distance(args.interactorObject.transform.position, nockSocket.transform.position);
        float clampedPullDistance = Mathf.Clamp(pullDistance, 0f, maxPullDistance);
        float finalForce = clampedPullDistance * shootingForceMultiplier;

        if (enableDebugLogs)
            Debug.Log($"당김 거리: {pullDistance}, 클램프된 거리: {clampedPullDistance}, 최종 힘: {finalForce}");

        // 화살을 소켓에서 분리하고 물리 활성화
        Rigidbody arrowRigidbody = nockedArrow.transform.GetComponent<Rigidbody>();
        if (arrowRigidbody != null)
        {
            arrowRigidbody.isKinematic = false;
            arrowRigidbody.useGravity = true;

            // 화살에 전방 방향으로 힘을 가합니다.
            Vector3 shootDirection = nockedArrow.transform.forward;
            arrowRigidbody.AddForce(shootDirection * finalForce, ForceMode.Impulse);

            if (enableDebugLogs)
                Debug.Log($"화살에 가한 힘: {shootDirection * finalForce}");
        }
        else
        {
            Debug.LogError("화살에 Rigidbody가 없습니다!");
        }
    }

    /// <summary>
    /// 화살 발사 (개선된 버전)
    /// </summary>
    /// <param name="pullStrength">당김 강도 (0-1)</param>
    private void FireArrow(float pullStrength)
    {
        if (nockedArrow == null) return;

        // ArrowLauncher 컴포넌트 찾기
        ArrowLauncher launcher = nockedArrow.transform.GetComponent<ArrowLauncher>();
        if (launcher != null)
        {
            // ArrowLauncher의 Release 함수 호출
            // 이 함수는 내부적으로 화살 발사 로직을 처리합니다
            launcher.Initialize(stringPullInteractable);
        }
        else
        {
            // 기존 방식으로 발사 (호환성 유지)
            Rigidbody arrowRigidbody = nockedArrow.transform.GetComponent<Rigidbody>();
            if (arrowRigidbody != null)
            {
                arrowRigidbody.isKinematic = false;
                arrowRigidbody.useGravity = true;

                Vector3 shootDirection = nockedArrow.transform.forward;
                float finalForce = pullStrength * shootingForceMultiplier;
                arrowRigidbody.AddForce(shootDirection * finalForce, ForceMode.Impulse);
            }
        }

        // 화살 상태 리셋
        nockedArrow = null;
        isArrowNocked = false;
    }

    // 시위(Line Renderer)를 기본 위치로 되돌림
    private void ResetBowString()
    {
        if (bowStringRenderer != null)
        {
            bowStringRenderer.positionCount = 2;
            bowStringRenderer.SetPosition(0, stringStartPoint.position);
            bowStringRenderer.SetPosition(1, stringEndPoint.position);
        }
    }

    // 시위를 당긴 손 위치로 업데이트
    private void UpdateBowString(Vector3 pullPosition)
    {
        if (bowStringRenderer != null)
        {
            // 당김 거리를 계산
            Vector3 direction = (pullPosition - nockSocket.transform.position).normalized;
            float distance = Vector3.Distance(nockSocket.transform.position, pullPosition);
            float clampedDistance = Mathf.Clamp(distance, 0f, maxPullDistance);
            Vector3 clampedPullPosition = nockSocket.transform.position + direction * clampedDistance;

            bowStringRenderer.positionCount = 3;
            bowStringRenderer.SetPosition(0, stringStartPoint.position);
            bowStringRenderer.SetPosition(1, clampedPullPosition);
            bowStringRenderer.SetPosition(2, stringEndPoint.position);

            // 당김 강도 이벤트 호출
            float pullStrength = clampedDistance / maxPullDistance;
            OnPullStrengthChanged?.Invoke(pullStrength);
        }
    }

    // 인스펙터에서 화살 생성 (테스트용)
    [ContextMenu("Create Arrow")]
    public void CreateArrow()
    {
        if (arrowPrefab != null && arrowSpawnPoint != null)
        {
            GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
            SetupArrowComponents(newArrow);
            Debug.Log("화살이 수동으로 생성되었습니다.");
        }
        else
        {
            Debug.LogError("화살 프리팹이나 생성 위치가 설정되지 않았습니다!");
        }
    }

    // 상태 정보 반환
    public bool IsArrowNocked() => isArrowNocked;
    
    public float GetPullDistance()
    {
        if (isArrowNocked && nockedArrow != null)
        {
            IXRSelectInteractor hand = nockedArrow.firstInteractorSelecting;
            if (hand != null)
            {
                return Vector3.Distance(hand.transform.position, nockSocket.transform.position);
            }
        }
        return 0f;
    }

    public int GetCurrentArrowCount() => currentArrowCount;
    
    public float GetMaxPullDistance() => maxPullDistance;
}