using UnityEngine;
using System.Collections;

/// <summary>
/// Ȱ��� ���ӿ� Ÿ�� ��Ʈ�ѷ�
/// Ÿ���� ����, �ı�, �ִϸ��̼��� ����
/// </summary>
public class TargetController : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Ÿ���� �ִ� ü��")]
    public int maxHealth = 100;

    [Tooltip("Ÿ���� ������ �� ��� ����")]
    public int hitScore = 10;

    [Tooltip("Ÿ���� �ı����� �� ��� �߰� ����")]
    public int destroyScore = 50;

    [Header("Target Behavior")]
    [Tooltip("Ÿ���� �����̴��� ����")]
    public bool isMoving = false;

    [Tooltip("Ÿ���� �̵� �ӵ�")]
    public float moveSpeed = 2f;

    [Tooltip("Ÿ���� �̵� ����")]
    public float moveRange = 5f;

    [Tooltip("Ÿ���� ȸ���ϴ��� ����")]
    public bool isRotating = false;

    [Tooltip("Ÿ���� ȸ�� �ӵ�")]
    public float rotationSpeed = 30f;

    [Header("Visual Effects")]
    [Tooltip("Ÿ���� �¾��� ���� ����Ʈ")]
    public GameObject hitEffectPrefab;

    [Tooltip("Ÿ���� �ı��� ���� ����Ʈ")]
    public GameObject destroyEffectPrefab;

    [Tooltip("Ÿ���� ���� ����")]
    public Color originalColor = Color.white;

    [Tooltip("Ÿ���� �¾��� ���� ����")]
    public Color hitColor = Color.red;

    [Header("Audio")]
    [Tooltip("Ÿ���� �¾��� ���� �Ҹ�")]
    public AudioClip hitSound;

    [Tooltip("Ÿ���� �ı��� ���� �Ҹ�")]
    public AudioClip destroySound;

    // ���� ������
    private int _currentHealth;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Renderer _renderer;
    private AudioSource _audioSource;
    private bool _isDestroyed = false;

    // �̺�Ʈ
    public System.Action<int> OnTargetHit;
    public System.Action OnTargetDestroyed;

    void Start()
    {
        InitializeTarget();
    }

    /// <summary>
    /// Ÿ�� �ʱ�ȭ
    /// </summary>
    void InitializeTarget()
    {
        _currentHealth = maxHealth;
        _startPosition = transform.position;
        _startRotation = transform.rotation;

        // ������ ��������
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            _renderer.material.color = originalColor;
        }

        // AudioSource ����
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        // �±� ����
        gameObject.tag = "Target";
    }

    void Update()
    {
        if (!_isDestroyed)
        {
            UpdateMovement();
            UpdateRotation();
        }
    }

    /// <summary>
    /// Ÿ�� �̵� ������Ʈ
    /// </summary>
    void UpdateMovement()
    {
        if (isMoving)
        {
            float time = Time.time * moveSpeed;
            float offset = Mathf.Sin(time) * moveRange;
            Vector3 newPosition = _startPosition + Vector3.right * offset;
            transform.position = newPosition;
        }
    }

    /// <summary>
    /// Ÿ�� ȸ�� ������Ʈ
    /// </summary>
    void UpdateRotation()
    {
        if (isRotating)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Ÿ���� �¾��� �� ȣ��
    /// </summary>
    /// <param name="damage">���� ������</param>
    public void TakeDamage(int damage)
    {
        if (_isDestroyed) return;

        _currentHealth -= damage;

        // ��Ʈ �̺�Ʈ ȣ��
        OnTargetHit?.Invoke(damage);

        // ��Ʈ ����Ʈ ����
        if (hitEffectPrefab != null)
        {
            GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(hitEffect, 2f);
        }

        // ��Ʈ ���� ���
        if (hitSound != null)
        {
            _audioSource.PlayOneShot(hitSound);
        }

        // �ð��� �ǵ��
        StartCoroutine(HitVisualFeedback());

        // ü���� 0 ���ϰ� �Ǹ� �ı�
        if (_currentHealth <= 0)
        {
            DestroyTarget();
        }
    }

    /// <summary>
    /// Ÿ�� �ı�
    /// </summary>
    void DestroyTarget()
    {
        if (_isDestroyed) return;

        _isDestroyed = true;

        // �ı� �̺�Ʈ ȣ��
        OnTargetDestroyed?.Invoke();

        // �ı� ����Ʈ ����
        if (destroyEffectPrefab != null)
        {
            GameObject destroyEffect = Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
            Destroy(destroyEffect, 3f);
        }

        // �ı� ���� ���
        if (destroySound != null)
        {
            _audioSource.PlayOneShot(destroySound);
        }

        // ���� �߰�
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(destroyScore);
        }

        // Ÿ�� ��Ȱ��ȭ (����Ʈ ��� ��)
        StartCoroutine(DestroyAfterDelay(2f));
    }

    /// <summary>
    /// ��Ʈ �ð��� �ǵ��
    /// </summary>
    IEnumerator HitVisualFeedback()
    {
        if (_renderer != null)
        {
            Color originalColor = _renderer.material.color;
            _renderer.material.color = hitColor;

            yield return new WaitForSeconds(0.1f);

            _renderer.material.color = originalColor;
        }
    }

    /// <summary>
    /// ���� �� �ı�
    /// </summary>
    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Ÿ�� ����
    /// </summary>
    public void ResetTarget()
    {
        _isDestroyed = false;
        _currentHealth = maxHealth;
        transform.position = _startPosition;
        transform.rotation = _startRotation;

        if (_renderer != null)
        {
            _renderer.material.color = originalColor;
        }

        gameObject.SetActive(true);
    }

    /// <summary>
    /// ���� ü�� ��ȯ
    /// </summary>
    public int GetCurrentHealth()
    {
        return _currentHealth;
    }

    /// <summary>
    /// ü�� ���� ��ȯ (0-1)
    /// </summary>
    public float GetHealthRatio()
    {
        return (float)_currentHealth / maxHealth;
    }

    /// <summary>
    /// Ÿ���� �ı��Ǿ����� Ȯ��
    /// </summary>
    public bool IsDestroyed()
    {
        return _isDestroyed;
    }
}