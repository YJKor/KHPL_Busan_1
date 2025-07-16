using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 화살의 충돌 처리, 데미지, 이펙트를 담당하는 컴포넌트
/// ArrowLauncher와 함께 사용되어 완전한 활쏘기 시스템을 구성합니다.
/// </summary>
public class ArrowImpactHandler : MonoBehaviour
{
    [Header("Arrow Settings")]
    [Tooltip("화살이 주는 기본 데미지")]
    [SerializeField] private int damage = 15;

    [Tooltip("화살이 충돌 후 파괴되는 시간 (초)")]
    [SerializeField] private float destroyDelay = 3f;

    [Header("Effects")]
    [Tooltip("화살이 타겟에 맞았을 때 재생할 이펙트 프리팹")]
    [SerializeField] private GameObject hitEffectPrefab;

    [Tooltip("타겟이 파괴될 때 재생할 이펙트 프리팹")]
    [SerializeField] private GameObject destroyEffectPrefab;

    [Tooltip("화살이 타겟에 맞았을 때 재생할 사운드")]
    [SerializeField] private AudioClip hitSound;

    [Tooltip("타겟이 파괴될 때 재생할 사운드")]
    [SerializeField] private AudioClip destroySound;

    [Header("Score")]
    [Tooltip("타겟을 맞췄을 때 기본 점수")]
    [SerializeField] private int hitScore = 10;

    [Tooltip("타겟을 파괴했을 때 추가 점수")]
    [SerializeField] private int destroyScore = 50;

    [Header("Physics")]
    [Tooltip("충돌 후 화살의 물리 효과 비활성화")]
    [SerializeField] private bool disablePhysicsOnHit = true;

    // 내부 변수
    private bool _hasHit = false;
    private AudioSource _audioSource;
    private Rigidbody _rigidbody;
    private ArrowLauncher _arrowLauncher;

    void Start()
    {
        InitializeComponents();
    }

    /// <summary>
    /// 필요한 컴포넌트들을 초기화합니다.
    /// </summary>
    private void InitializeComponents()
    {
        // AudioSource 컴포넌트 찾기 또는 추가
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Rigidbody 컴포넌트 찾기
        _rigidbody = GetComponent<Rigidbody>();

        // ArrowLauncher 컴포넌트 찾기
        _arrowLauncher = GetComponent<ArrowLauncher>();
    }

    /// <summary>
    /// 다른 오브젝트와 충돌했을 때 호출되는 함수
    /// 충돌 처리, 이펙트 재생, 점수 처리, 파괴 로직을 담당합니다.
    /// </summary>
    /// <param name="collision">충돌 정보를 담고 있는 Collision 객체</param>
    void OnCollisionEnter(Collision collision)
    {
        // 이미 충돌한 화살은 중복 처리 방지
        if (_hasHit) return;

        _hasHit = true;

        // 화살 비행 중지 (ArrowLauncher와 연동)
        if (_arrowLauncher != null)
        {
            _arrowLauncher.StopFlight();
        }

        // 물리 효과 비활성화
        if (disablePhysicsOnHit && _rigidbody != null)
        {
            _rigidbody.isKinematic = true;
        }

        // 충돌 지점에 히트 이펙트 생성
        CreateHitEffect(collision);

        // 히트 사운드 재생
        PlayHitSound();

        // 점수 처리
        ProcessScore(collision.gameObject);

        // 타겟 처리 (데미지, 파괴 등)
        ProcessTarget(collision.gameObject);

        // 화살 파괴 (지연 시간 후)
        Destroy(gameObject, destroyDelay);
    }

    /// <summary>
    /// 충돌 지점에 히트 이펙트를 생성합니다.
    /// </summary>
    /// <param name="collision">충돌 정보</param>
    private void CreateHitEffect(Collision collision)
    {
        if (hitEffectPrefab != null)
        {
            Vector3 hitPoint = collision.contacts[0].point;
            Quaternion hitRotation = Quaternion.LookRotation(collision.contacts[0].normal);
            GameObject hitEffect = Instantiate(hitEffectPrefab, hitPoint, hitRotation);
            Destroy(hitEffect, 3f);
        }
    }

    /// <summary>
    /// 히트 사운드를 재생합니다.
    /// </summary>
    private void PlayHitSound()
    {
        if (hitSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(hitSound);
        }
    }

    /// <summary>
    /// 충돌한 타겟에 대한 점수 처리를 담당하는 함수
    /// 타겟의 태그에 따라 다른 점수를 부여합니다.
    /// </summary>
    /// <param name="target">충돌한 타겟 오브젝트</param>
    private void ProcessScore(GameObject target)
    {
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            if (target.CompareTag("Enemy"))
            {
                scoreManager.AddScore(hitScore);
            }
            else if (target.CompareTag("Target"))
            {
                scoreManager.AddScore(hitScore);
            }
            else
            {
                scoreManager.AddScore(5); // 기본 점수 (기타 오브젝트)
            }
        }
    }

    /// <summary>
    /// 충돌한 타겟에 대한 데미지 및 파괴 처리를 담당하는 함수
    /// 다양한 타겟 시스템과 호환됩니다.
    /// </summary>
    /// <param name="target">충돌한 타겟 오브젝트</param>
    private void ProcessTarget(GameObject target)
    {
        // EnemyController 컴포넌트 확인
        EnemyController enemy = target.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            CreateDestroyEffect(target.transform.position);
            PlayDestroySound();
            AddDestroyScore();
            return;
        }

        // DestructibleObject 컴포넌트 확인
        DestructibleObject destructible = target.GetComponent<DestructibleObject>();
        if (destructible != null)
        {
            destructible.TakeDamage(damage);
            CreateDestroyEffect(target.transform.position);
            PlayDestroySound();
            AddDestroyScore();
            return;
        }

        // HealthSystem 컴포넌트 확인
        HealthSystem healthSystem = target.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.TakeDamage(damage);
        }
    }

    /// <summary>
    /// 파괴 이펙트를 생성합니다.
    /// </summary>
    /// <param name="position">이펙트 생성 위치</param>
    private void CreateDestroyEffect(Vector3 position)
    {
        if (destroyEffectPrefab != null)
        {
            GameObject destroyEffect = Instantiate(destroyEffectPrefab, position, Quaternion.identity);
            Destroy(destroyEffect, 5f);
        }
    }

    /// <summary>
    /// 파괴 사운드를 재생합니다.
    /// </summary>
    private void PlayDestroySound()
    {
        if (destroySound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(destroySound);
        }
    }

    /// <summary>
    /// 파괴 점수를 추가합니다.
    /// </summary>
    private void AddDestroyScore()
    {
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(destroyScore);
        }
    }

    /// <summary>
    /// 화살이 카메라 밖으로 나갔을 때 호출되는 함수
    /// 메모리 관리를 위해 화살을 파괴합니다.
    /// </summary>
    void OnBecameInvisible()
    {
        if (!_hasHit)
        {
            Destroy(gameObject, 2f);
        }
    }

    /// <summary>
    /// 화살이 이미 충돌했는지 확인하는 프로퍼티
    /// </summary>
    public bool HasHit => _hasHit;

    /// <summary>
    /// 화살의 데미지를 설정합니다.
    /// </summary>
    /// <param name="newDamage">새로운 데미지 값</param>
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    /// <summary>
    /// 현재 화살의 데미지를 반환합니다.
    /// </summary>
    /// <returns>현재 데미지 값</returns>
    public int GetDamage()
    {
        return damage;
    }
} 