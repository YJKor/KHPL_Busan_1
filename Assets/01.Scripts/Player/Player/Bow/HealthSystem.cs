using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ������Ʈ�� ü�� �ý����� �����ϴ� ��ũ��Ʈ
/// ������, ����, �̺�Ʈ �ý���, ����Ʈ ����� �����մϴ�.
/// </summary>
public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("������Ʈ�� �ִ� ü��")]
    public int maxHealth = 100;

    [Tooltip("������Ʈ�� ���� ü��")]
    public int currentHealth;

    [Header("Events")]
    [Tooltip("�������� �޾��� �� ȣ��� �̺�Ʈ")]
    public UnityEvent OnDamage;

    [Tooltip("������� �� ȣ��� �̺�Ʈ")]
    public UnityEvent OnDeath;

    [Tooltip("ü���� ����Ǿ��� �� ȣ��� �̺�Ʈ (���� ü���� �Ű������� ����)")]
    public UnityEvent<int> OnHealthChanged;

    [Header("Effects")]
    [Tooltip("�������� �޾��� �� ������ ����Ʈ ������")]
    public GameObject damageEffectPrefab;

    [Tooltip("������� �� ������ ����Ʈ ������")]
    public GameObject deathEffectPrefab;

    [Tooltip("�������� �޾��� �� ����� ����")]
    public AudioClip damageSound;

    [Tooltip("������� �� ����� ����")]
    public AudioClip deathSound;

    // ���� ������
    /// <summary>���� ����� ���� AudioSource ������Ʈ</summary>
    private AudioSource _audioSource;

    /// <summary>������Ʈ�� �̹� ����ߴ��� Ȯ���ϴ� �÷���</summary>
    private bool _isDead = false;

    /// <summary>
    /// ��ũ��Ʈ �ʱ�ȭ �� ȣ��Ǵ� �Լ�
    /// ü���� �ִ밪���� �����ϰ� AudioSource�� �غ��ϸ� �ʱ� �̺�Ʈ�� ȣ���մϴ�.
    /// </summary>
    void Start()
    {
        currentHealth = maxHealth;
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        // �ʱ� ü�� ���� �̺�Ʈ ȣ��
        OnHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// ������Ʈ�� �������� �޾��� �� ȣ��Ǵ� �Լ�
    /// ü���� ���ҽ�Ű�� ����Ʈ�� �����ϸ�, ü���� 0 ���ϰ� �Ǹ� ��� ó���մϴ�.
    /// </summary>
    /// <param name="damage">���� ��������</param>
    public void TakeDamage(int damage)
    {
        // �̹� ����� ������Ʈ�� �������� ���� ����
        if (_isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // ü���� ������ ���� �ʵ��� ����

        // ������ ����Ʈ ����
        if (damageEffectPrefab != null)
        {
            GameObject damageEffect = Instantiate(damageEffectPrefab, transform.position, Quaternion.identity);
            Destroy(damageEffect, 2f); // 2�� �� ����Ʈ ����
        }

        // ������ ���� ���
        if (damageSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(damageSound);
        }

        // �̺�Ʈ ȣ��
        OnDamage?.Invoke();
        OnHealthChanged?.Invoke(currentHealth);

        // ü���� 0 ���ϰ� �Ǹ� ���
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// ������Ʈ�� ü���� ȸ���ϴ� �Լ�
    /// ������ �縸ŭ ü���� ������Ű�� �ִ� ü���� �ʰ����� �ʵ��� �����մϴ�.
    /// </summary>
    /// <param name="healAmount">ȸ���� ü�·�</param>
    public void Heal(int healAmount)
    {
        // �̹� ����� ������Ʈ�� ȸ���� �� ����
        if (_isDead) return;

        currentHealth += healAmount;
        currentHealth = Mathf.Min(maxHealth, currentHealth); // �ִ� ü���� �ʰ����� �ʵ��� ����

        OnHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// ������Ʈ�� ü���� ���� �����ϴ� �Լ�
    /// 0���� �ִ� ü�� ������ ������ ���ѵ˴ϴ�.
    /// </summary>
    /// <param name="health">������ ü�°�</param>
    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);

        // ü���� 0 ���ϰ� �ǰ� ���� ������� �ʾҴٸ� ��� ó��
        if (currentHealth <= 0 && !_isDead)
        {
            Die();
        }
    }

    /// <summary>
    /// ������Ʈ�� ��� ó���ϴ� �Լ�
    /// ����Ʈ, ����, �̺�Ʈ�� �߻���Ű�� ��� ���·� �����մϴ�.
    /// </summary>
    void Die()
    {
        // �̹� ����� ������Ʈ�� �ߺ� ó�� ����
        if (_isDead) return;
        _isDead = true;

        // ��� ����Ʈ ����
        if (deathEffectPrefab != null)
        {
            GameObject deathEffect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(deathEffect, 5f); // 5�� �� ����Ʈ ����
        }

        // ��� ���� ���
        if (deathSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(deathSound);
        }

        // ��� �̺�Ʈ ȣ��
        OnDeath?.Invoke();

        // ������Ʈ �ı� (���û��� - �ּ� ó���Ǿ� ����)
        // Destroy(gameObject, 2f);
    }

    /// <summary>
    /// ���� ü���� ������� ��ȯ�ϴ� �Լ�
    /// UI ǥ�ó� ���� �������� ���˴ϴ�.
    /// </summary>
    /// <returns>ü�� ����� (0.0 ~ 1.0)</returns>
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    /// <summary>
    /// ������Ʈ�� ����ߴ��� Ȯ���ϴ� �Լ�
    /// �ٸ� ��ũ��Ʈ���� ��� ���¸� Ȯ���� �� ���˴ϴ�.
    /// </summary>
    /// <returns>��������� true, �ƴϸ� false</returns>
    public bool IsDead()
    {
        return _isDead;
    }

    /// <summary>
    /// ������Ʈ�� ü���� �ִ����� Ȯ���ϴ� �Լ�
    /// ���� �����̳� UI ǥ�ÿ��� ���˴ϴ�.
    /// </summary>
    /// <returns>ü���� �ִ��̸� true, �ƴϸ� false</returns>
    public bool IsFullHealth()
    {
        return currentHealth >= maxHealth;
    }

    /// <summary>
    /// ������Ʈ�� ��Ȱ��Ű�� �Լ�
    /// ü���� �ִ밪���� ȸ���ϰ� ��� ���¸� �����մϴ�.
    /// </summary>
    public void Revive()
    {
        if (_isDead)
        {
            _isDead = false;
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth);
        }
    }


    /// <summary>
    /// ������Ʈ�� ������ ȸ����Ű�� �Լ�
    /// ü���� �ִ밪���� �����մϴ� (��� ���°� �ƴ� ���).
    /// </summary>
    public void FullHeal()
    {
        if (!_isDead)
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth);
        }
    }

    /// <summary>
    /// �ִ� ü���� �����ϴ� �Լ�
    /// ���ο� �ִ� ü���� ���� ü�º��� ������ ���� ü�µ� �����˴ϴ�.
    /// </summary>
    /// <param name="newMaxHealth">���ο� �ִ� ü��</param>
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }
}