using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// VR ���潺 ������ ������ �����ϴ� ��ũ��Ʈ
/// Ashigaru �����տ� �����Ͽ� ������ �̵��ϴ� ������ �����մϴ�.
/// </summary>
public class EnemyController : MonoBehaviour
{
    [Header("Enemy Settings")]
    [Tooltip("������ �ִ� ü��")]
    public int maxHealth = 100;

    [Tooltip("������ �̵� �ӵ�")]
    public float moveSpeed = 3f;

    [Tooltip("������ ���� �������� �� ���ϴ� ������")]
    public int castleDamage = 20;

    [Header("AI Settings")]
    [Tooltip("��ǥ ���� (���� ��ġ)")]
    public Transform targetCastle;

    [Tooltip("������ ���� �������� ���� ���� �ð�")]
    public float attackDelay = 1f;

    [Header("Effects")]
    [Tooltip("������ ���ظ� �޾��� ���� ����Ʈ")]
    public GameObject hitEffectPrefab;

    [Tooltip("������ �׾��� ���� ����Ʈ")]
    public GameObject deathEffectPrefab;

    [Tooltip("������ ���� �������� ���� ����Ʈ")]
    public GameObject castleHitEffectPrefab;

    [Header("Audio")]
    [Tooltip("������ ���ظ� �޾��� ���� ����")]
    public AudioClip hitSound;

    [Tooltip("������ �׾��� ���� ����")]
    public AudioClip deathSound;

    [Tooltip("������ ���� �������� ���� ����")]
    public AudioClip castleHitSound;

    // ���� ������
    private int currentHealth;
    private NavMeshAgent navAgent;
    private AudioSource audioSource;
    private bool isDead = false;
    private bool hasReachedCastle = false;

    // �̺�Ʈ
    public System.Action<EnemyController> OnEnemyDeath;
    public System.Action<EnemyController> OnEnemyReachedCastle;

    void Start()
    {
        InitializeEnemy();
    }

    /// <summary>
    /// ���� �ʱ�ȭ
    /// </summary>
    void InitializeEnemy()
    {
        currentHealth = maxHealth;

        // NavMeshAgent ������Ʈ ��������
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            navAgent = gameObject.AddComponent<NavMeshAgent>();
        }

        // AudioSource ������Ʈ ��������
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // NavMeshAgent ����
        navAgent.speed = moveSpeed;
        navAgent.stoppingDistance = 1f;

        // ��ǥ ���� ����
        if (targetCastle != null)
        {
            navAgent.SetDestination(targetCastle.position);
        }
        else
        {
            // ������ "Castle" �±׸� ���� ������Ʈ ã��
            GameObject castle = GameObject.FindGameObjectWithTag("Castle");
            if (castle != null)
            {
                targetCastle = castle.transform;
                navAgent.SetDestination(targetCastle.position);
            }
        }
    }

    void Update()
    {
        if (isDead || hasReachedCastle) return;

        // ���� �����ߴ��� Ȯ��
        if (targetCastle != null && Vector3.Distance(transform.position, targetCastle.position) <= navAgent.stoppingDistance)
        {
            OnReachCastle();
        }
    }

    /// <summary>
    /// ������ �������� �޾��� �� ȣ��
    /// </summary>
    /// <param name="damage">���� ��������</param>
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // ��Ʈ ����Ʈ ����
        if (hitEffectPrefab != null)
        {
            GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position + Vector3.up, Quaternion.identity);
            Destroy(hitEffect, 2f);
        }

        // ��Ʈ ���� ���
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // ü���� 0 ���ϰ� �Ǹ� ����
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// ������ �׾��� �� ȣ��
    /// </summary>
    void Die()
    {
        if (isDead) return;

        isDead = true;

        // ���� ����Ʈ ����
        if (deathEffectPrefab != null)
        {
            GameObject deathEffect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(deathEffect, 3f);
        }

        // ���� ���� ���
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // AI ��Ȱ��ȭ
        if (navAgent != null)
        {
            navAgent.enabled = false;
        }

        // �̺�Ʈ ȣ��
        OnEnemyDeath?.Invoke(this);

        // ������Ʈ �ı� (���� �ð� ��)
        Destroy(gameObject, 2f);
    }

    /// <summary>
    /// ������ ���� �������� �� ȣ��
    /// </summary>
    void OnReachCastle()
    {
        if (hasReachedCastle) return;

        hasReachedCastle = true;

        // AI ��Ȱ��ȭ
        if (navAgent != null)
        {
            navAgent.enabled = false;
        }

        // �� ���� ����Ʈ ����
        if (castleHitEffectPrefab != null)
        {
            GameObject castleEffect = Instantiate(castleHitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(castleEffect, 3f);
        }

        // �� ���� ���� ���
        if (castleHitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(castleHitSound);
        }

        // ���� ������ ����
        CastleHealth castleHealth = targetCastle.GetComponent<CastleHealth>();
        if (castleHealth != null)
        {
            castleHealth.TakeDamage(castleDamage);
        }

        // �̺�Ʈ ȣ��
        OnEnemyReachedCastle?.Invoke(this);

        // ������Ʈ �ı�
        Destroy(gameObject, 1f);
    }

    /// <summary>
    /// ���� ü�� ��ȯ
    /// </summary>
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// �ִ� ü�� ��ȯ
    /// </summary>
    public int GetMaxHealth()
    {
        return maxHealth;
    }

    /// <summary>
    /// ü�� ���� ��ȯ (0~1)
    /// </summary>
    public float GetHealthRatio()
    {
        return (float)currentHealth / maxHealth;
    }

    /// <summary>
    /// ������ �׾����� Ȯ��
    /// </summary>
    public bool IsDead()
    {
        return isDead;
    }

    /// <summary>
    /// ������ ���� �����ߴ��� Ȯ��
    /// </summary>
    public bool HasReachedCastle()
    {
        return hasReachedCastle;
    }
}
