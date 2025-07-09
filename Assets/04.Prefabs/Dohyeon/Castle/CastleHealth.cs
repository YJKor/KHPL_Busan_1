using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ���� ü���� �����ϰ� ���� ������ ó���ϴ� ��ũ��Ʈ
/// </summary>
public class CastleHealth : MonoBehaviour
{
    [Header("Castle Settings")]
    [Tooltip("���� �ִ� ü��")]
    public int maxHealth = 1000;

    [Tooltip("���� ���� ü��")]
    [SerializeField] private int currentHealth;

    [Header("Events")]
    [Tooltip("���� ü���� ����Ǿ��� �� ȣ��Ǵ� �̺�Ʈ")]
    public UnityEvent<int, int> OnHealthChanged; // currentHealth, maxHealth

    [Tooltip("���� ü���� 0�� �Ǿ��� �� ȣ��Ǵ� �̺�Ʈ")]
    public UnityEvent OnGameOver;

    [Header("Effects")]
    [Tooltip("���� �������� �޾��� ���� ����Ʈ")]
    public GameObject damageEffectPrefab;

    [Tooltip("���� ü���� ���� ���� ��� ����Ʈ")]
    public GameObject warningEffectPrefab;

    [Header("Audio")]
    [Tooltip("���� �������� �޾��� ���� ����")]
    public AudioClip damageSound;

    [Tooltip("���� ���� ����")]
    public AudioClip gameOverSound;

    // ���� ������
    private AudioSource audioSource;
    private bool isGameOver = false;
    private GameObject warningEffect;

    // ������Ƽ
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public float HealthRatio => (float)currentHealth / maxHealth;
    public bool IsGameOver => isGameOver;

    void Start()
    {
        InitializeCastle();
    }

    /// <summary>
    /// �� �ʱ�ȭ
    /// </summary>
    void InitializeCastle()
    {
        currentHealth = maxHealth;

        // AudioSource ������Ʈ ��������
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // �̺�Ʈ ȣ��
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// ���� �������� �޾��� �� ȣ��
    /// </summary>
    /// <param name="damage">���� ��������</param>
    public void TakeDamage(int damage)
    {
        if (isGameOver) return;

        currentHealth -= damage;

        // ü���� 0 ���ϰ� �Ǹ� 0���� ����
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        // ������ ����Ʈ ����
        if (damageEffectPrefab != null)
        {
            GameObject damageEffect = Instantiate(damageEffectPrefab, transform.position, Quaternion.identity);
            Destroy(damageEffect, 2f);
        }

        // ������ ���� ���
        if (damageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        // ü�� ���� �̺�Ʈ ȣ��
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // ü���� ���� �� ��� ����Ʈ ǥ��
        CheckWarningEffect();

        // ü���� 0�� �Ǹ� ���� ����
        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    /// <summary>
    /// ���� ü���� ȸ��
    /// </summary>
    /// <param name="healAmount">ȸ����</param>
    public void Heal(int healAmount)
    {
        if (isGameOver) return;

        currentHealth += healAmount;

        // ü���� �ִ�ġ�� ���� �ʵ��� ����
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // ü�� ���� �̺�Ʈ ȣ��
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // ��� ����Ʈ üũ
        CheckWarningEffect();
    }

    /// <summary>
    /// ü���� ���� �� ��� ����Ʈ ǥ��
    /// </summary>
    void CheckWarningEffect()
    {
        float healthRatio = HealthRatio;

        // ü���� 30% ������ �� ��� ����Ʈ ǥ��
        if (healthRatio <= 0.3f && warningEffect == null && warningEffectPrefab != null)
        {
            warningEffect = Instantiate(warningEffectPrefab, transform.position, Quaternion.identity);
        }
        // ü���� 30% �̻��� �� ��� ����Ʈ ����
        else if (healthRatio > 0.3f && warningEffect != null)
        {
            Destroy(warningEffect);
            warningEffect = null;
        }
    }

    /// <summary>
    /// ���� ���� ó��
    /// </summary>
    void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        // ���� ���� ���� ���
        if (gameOverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        // ��� ����Ʈ ����
        if (warningEffect != null)
        {
            Destroy(warningEffect);
            warningEffect = null;
        }

        // ���� ���� �̺�Ʈ ȣ��
        OnGameOver?.Invoke();

        Debug.Log("���� ����! ���� �ı��Ǿ����ϴ�.");
    }

    /// <summary>
    /// ���� ü���� ������ ȸ��
    /// </summary>
    public void RestoreFullHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        CheckWarningEffect();
    }

    /// <summary>
    /// ���� �ִ� ü�� ����
    /// </summary>
    /// <param name="newMaxHealth">���ο� �ִ� ü��</param>
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void OnDestroy()
    {
        // ��� ����Ʈ ����
        if (warningEffect != null)
        {
            Destroy(warningEffect);
        }
    }
}