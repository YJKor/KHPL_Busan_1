using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ı� ������ ������Ʈ�� �����ϴ� ��ũ��Ʈ
/// ü�� �ý���, �ı� ����Ʈ, ���� ���� ����� �����մϴ�.
/// </summary>
public class DestructibleObject : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("������Ʈ�� �ִ� ü��")]
    public int maxHealth = 100;

    [Tooltip("������Ʈ�� ���� ü��")]
    public int currentHealth;

    [Header("Effects")]
    [Tooltip("�������� �޾��� �� ������ ����Ʈ ������")]
    public GameObject hitEffectPrefab;

    [Tooltip("�ı��� �� ������ ����Ʈ ������")]
    public GameObject destroyEffectPrefab;

    [Tooltip("�������� �޾��� �� ����� ����")]
    public AudioClip hitSound;

    [Tooltip("�ı��� �� ����� ����")]
    public AudioClip destroySound;

    [Header("Destruction")]
    [Tooltip("�ı� �� ������ ���� �����յ��� �迭")]
    public GameObject[] debrisPrefabs;

    [Tooltip("�ı� �� ������ ������ ����")]
    public int debrisCount = 5;

    [Tooltip("���ؿ� ����� ���߷��� ����")]
    public float explosionForce = 500f;

    [Tooltip("���߷��� ����Ǵ� �ݰ�")]
    public float explosionRadius = 3f;

    // ���� ������
    /// <summary>���� ����� ���� AudioSource ������Ʈ</summary>
    private AudioSource _audioSource;

    /// <summary>������Ʈ�� �̹� �ı��Ǿ����� Ȯ���ϴ� �÷���</summary>
    private bool _isDestroyed = false;

    /// <summary>
    /// ��ũ��Ʈ �ʱ�ȭ �� ȣ��Ǵ� �Լ�
    /// ü���� �ִ밪���� �����ϰ� AudioSource�� �غ��մϴ�.
    /// </summary>
    void Start()
    {
        currentHealth = maxHealth;
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// ������Ʈ�� �������� �޾��� �� ȣ��Ǵ� �Լ�
    /// ü���� ���ҽ�Ű�� ����Ʈ�� �����ϸ�, ü���� 0 ���ϰ� �Ǹ� �ı��մϴ�.
    /// </summary>
    /// <param name="damage">���� ��������</param>
    public void TakeDamage(int damage)
    {
        // �̹� �ı��� ������Ʈ�� �������� ���� ����
        if (_isDestroyed) return;

        currentHealth -= damage;

        // ��Ʈ ����Ʈ ����
        if (hitEffectPrefab != null)
        {
            GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(hitEffect, 2f); // 2�� �� ����Ʈ ����
        }

        // ��Ʈ ���� ���
        if (hitSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(hitSound);
        }

        // ü���� 0 ���ϰ� �Ǹ� �ı�
        if (currentHealth <= 0)
        {
            DestroyObject();
        }
    }

    /// <summary>
    /// ������Ʈ�� �ı��ϴ� �Լ�
    /// ����Ʈ, ����, ���� ������ �����ϰ� ������Ʈ�� �����մϴ�.
    /// </summary>
    void DestroyObject()
    {
        // �̹� �ı��� ������Ʈ�� �ߺ� ó�� ����
        if (_isDestroyed) return;
        _isDestroyed = true;

        // �ı� ����Ʈ ����
        if (destroyEffectPrefab != null)
        {
            GameObject destroyEffect = Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
            Destroy(destroyEffect, 5f); // 5�� �� ����Ʈ ����
        }

        // �ı� ���� ���
        if (destroySound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(destroySound);
        }

        // ���� ����
        CreateDebris();

        // ������Ʈ �ı� (0.1�� �������� ����Ʈ�� ������ ����ǵ��� ��)
        Destroy(gameObject, 0.1f);
    }

    /// <summary>
    /// �ı� �� ���ظ� �����ϴ� �Լ�
    /// ������ ��ġ�� ȸ������ ���ظ� �����ϰ� ���� ȿ���� �����մϴ�.
    /// </summary>
    void CreateDebris()
    {
        // ���� �������� �������� �ʾ����� �������� ����
        if (debrisPrefabs.Length == 0) return;

        for (int i = 0; i < debrisCount; i++)
        {
            // ������ ���� ������ ����
            GameObject debrisPrefab = debrisPrefabs[Random.Range(0, debrisPrefabs.Length)];

            // ������ ��ġ�� ���� (������Ʈ �߽ɿ��� 0.5 ���� �ݰ� ��)
            Vector3 randomPosition = transform.position + Random.insideUnitSphere * 0.5f;
            GameObject debris = Instantiate(debrisPrefab, randomPosition, Random.rotation);

            // ���� ȿ�� ����
            Rigidbody rb = debris.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // ���� �ڵ� �ı� (10�� ��)
            Destroy(debris, 10f);
        }
    }

    /// <summary>
    /// Ʈ���� �浹�� ���� �������� �޴� �Լ�
    /// "Arrow" �±׸� ���� ������Ʈ�� �浹 �� �������� �޽��ϴ�.
    /// </summary>
    /// <param name="other">�浹�� �ٸ� Collider</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Arrow") && !_isDestroyed)
        {
            ArrowController arrow = other.GetComponent<ArrowController>();
            if (arrow != null)
            {
                TakeDamage(arrow.damage);
            }
        }
    }

    /// <summary>
    /// ���� ü�� ������� ��ȯ�ϴ� �Լ�
    /// UI ǥ�ó� ���� �������� ���˴ϴ�.
    /// </summary>
    /// <returns>ü�� ����� (0.0 ~ 1.0)</returns>
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    /// <summary>
    /// ������Ʈ�� �ı��Ǿ����� Ȯ���ϴ� �Լ�
    /// �ٸ� ��ũ��Ʈ���� �ı� ���¸� Ȯ���� �� ���˴ϴ�.
    /// </summary>
    /// <returns>�ı��Ǿ����� true, �ƴϸ� false</returns>
    public bool IsDestroyed()
    {
        return _isDestroyed;
    }

    /// <summary>
    /// ������Ʈ�� ü���� ������ ȸ���ϴ� �Լ�
    /// ���� �������� �ʿ��� �� ���˴ϴ�.
    /// </summary>
    public void RestoreHealth()
    {
        if (!_isDestroyed)
        {
            currentHealth = maxHealth;
        }
    }
}