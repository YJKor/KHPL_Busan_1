using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ȭ���� �浹, �ı�, ����Ʈ, ���� ó���� ����ϴ� ��ũ��Ʈ
/// ȭ���� Ÿ�ٿ� �¾��� ���� ��� ������ ó���մϴ�.
/// </summary>
public class ArrowController : MonoBehaviour
{
    [Header("Arrow Settings")]
    [Tooltip("ȭ���� ���ϴ� ��������")]
    public int damage = 10;

    [Tooltip("ȭ���� �浹 �� �ı��Ǳ������ ���� �ð� (��)")]
    public float destroyDelay = 5f;

    [Header("Effects")]
    [Tooltip("ȭ���� Ÿ�ٿ� �¾��� �� ������ ����Ʈ ������")]
    public GameObject hitEffectPrefab;

    [Tooltip("Ÿ���� �ı��� �� ������ ����Ʈ ������")]
    public GameObject destroyEffectPrefab;

    [Tooltip("ȭ���� Ÿ�ٿ� �¾��� �� ����� ����")]
    public AudioClip hitSound;

    [Tooltip("Ÿ���� �ı��� �� ����� ����")]
    public AudioClip destroySound;

    [Header("Score")]
    [Tooltip("Ÿ���� ������ �� ��� �⺻ ����")]
    public int hitScore = 10;

    [Tooltip("Ÿ���� �ı����� �� ��� �߰� ����")]
    public int destroyScore = 50;

    // ���� ������
    /// <summary>ȭ���� �̹� �浹�ߴ��� Ȯ���ϴ� �÷���</summary>
    private bool _hasHit = false;

    /// <summary>���� ����� ���� AudioSource ������Ʈ</summary>
    private AudioSource _audioSource;

    /// <summary>���� ó���� ���� Rigidbody ������Ʈ</summary>
    private Rigidbody _rb;

    /// <summary>
    /// ��ũ��Ʈ �ʱ�ȭ �� ȣ��Ǵ� �Լ�
    /// AudioSource�� Rigidbody ������Ʈ�� �������ų� �߰��մϴ�.
    /// </summary>
    void Start()
    {
        // AudioSource ������Ʈ �������� (������ �߰�)
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Rigidbody ������Ʈ ��������
        _rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// ȭ���� �ٸ� ������Ʈ�� �浹���� �� ȣ��Ǵ� �Լ�
    /// �浹 ó��, ����Ʈ ����, ���� ó��, �ı� ������ �����մϴ�.
    /// </summary>
    /// <param name="collision">�浹 ������ ��� �ִ� Collision ��ü</param>
    void OnCollisionEnter(Collision collision)
    {
        // �̹� �浹�� ȭ���� �ߺ� ó�� ����
        if (!_hasHit)
        {
            _hasHit = true;

            // ȭ�� ���� ó�� (���� ȿ�� ��Ȱ��ȭ)
            if (_rb != null)
            {
                _rb.isKinematic = true;
            }

            // �浹 ������ ��Ʈ ����Ʈ ����
            if (hitEffectPrefab != null)
            {
                Vector3 hitPoint = collision.contacts[0].point; // �浹 ����
                Quaternion hitRotation = Quaternion.LookRotation(collision.contacts[0].normal); // �浹 ����
                GameObject hitEffect = Instantiate(hitEffectPrefab, hitPoint, hitRotation);
                Destroy(hitEffect, 3f); // 3�� �� ����Ʈ ����
            }

            // ��Ʈ ���� ���
            if (hitSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(hitSound);
            }

            // ���� ó��
            ProcessScore(collision.gameObject);

            // ��� ������Ʈ ó�� (������, �ı� ��)
            ProcessTarget(collision.gameObject);

            // ȭ�� �ı� (���� �ð� ��)
            Destroy(gameObject, destroyDelay);
        }
    }

    /// <summary>
    /// �浹�� Ÿ�ٿ� ���� ���� ó���� �����ϴ� �Լ�
    /// Ÿ���� �±׿� ���� �ٸ� ������ �ο��մϴ�.
    /// </summary>
    /// <param name="target">�浹�� Ÿ�� ������Ʈ</param>
    void ProcessScore(GameObject target)
    {
        // ������ ScoreManager ã��
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            // Ÿ���� �±׿� ���� �ٸ� ���� �ο�
            if (target.CompareTag("Enemy"))
            {
                scoreManager.AddScore(hitScore); // �� Ÿ��
            }
            else if (target.CompareTag("Target"))
            {
                scoreManager.AddScore(hitScore); // �Ϲ� Ÿ��
            }
            else
            {
                scoreManager.AddScore(5); // �⺻ ���� (��Ÿ ������Ʈ)
            }
        }
    }

    /// <summary>
    /// �浹�� Ÿ�ٿ� ���� ������ �� �ı� ó���� �����ϴ� �Լ�
    /// DestructibleObject�� HealthSystem�� �����մϴ�.
    /// </summary>
    /// <param name="target">�浹�� Ÿ�� ������Ʈ</param>
    void ProcessTarget(GameObject target)
    {
        // Ÿ���� �������� Ȯ�� (VR ���潺 ���ӿ�)
        EnemyController enemy = target.GetComponent<EnemyController>();
        if (enemy != null)
        {
            // ������ ������ ����
            enemy.TakeDamage(damage);

            // �ı� ����Ʈ ����
            if (destroyEffectPrefab != null)
            {
                Vector3 targetPosition = target.transform.position;
                GameObject destroyEffect = Instantiate(destroyEffectPrefab, targetPosition, Quaternion.identity);
                Destroy(destroyEffect, 5f);
            }

            // �ı� ���� ���
            if (destroySound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(destroySound);
            }

            // �߰� ���� (�ı� ���ʽ�)
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
            if (scoreManager != null)
            {
                scoreManager.AddScore(destroyScore);
            }

            return; // ���� ó���� �Ϸ�Ǿ����Ƿ� �ٸ� ó�� �ߴ�
        }
        //// Ÿ���� �ı� ������ ������Ʈ���� Ȯ��
        //DestructibleObject destructible = target.GetComponent<DestructibleObject>();
        //if (destructible != null)
        //{
        //    // ������ ����
        //    destructible.TakeDamage(damage);

        //    // �ı� ����Ʈ ����
        //    if (destroyEffectPrefab != null)
        //    {
        //        Vector3 targetPosition = target.transform.position;
        //        GameObject destroyEffect = Instantiate(destroyEffectPrefab, targetPosition, Quaternion.identity);
        //        Destroy(destroyEffect, 5f);
        //    }

        //    // �ı� ���� ���
        //    if (destroySound != null && _audioSource != null)
        //    {
        //        _audioSource.PlayOneShot(destroySound);
        //    }

        //    // �߰� ���� (�ı� ���ʽ�)
        //    ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        //    if (scoreManager != null)
        //    {
        //        scoreManager.AddScore(destroyScore);
        //    }
        //}

        // Ÿ���� ü�� �ý����� ���� ������Ʈ���� Ȯ��
        HealthSystem healthSystem = target.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.TakeDamage(damage);
        }
    }

    /// <summary>
    /// ȭ���� ȭ�� ������ ������ �� �ڵ����� �ı��ϴ� �Լ�
    /// �޸� ���� ������ ���� ����ȭ�� ���� ���˴ϴ�.
    /// </summary>
    void OnBecameInvisible()
    {
        // ���� �浹���� ���� ȭ�츸 �ı� (�浹�� ȭ���� �̹� �ı� ����)
        if (!_hasHit)
        {
            Destroy(gameObject, 2f); // 2�� �� �ı�
        }
    }
}