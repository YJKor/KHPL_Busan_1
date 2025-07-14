using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// ȭ���� XR ��ȣ�ۿ��� ����ϴ� ��ũ��Ʈ
/// XR Socket Interactor�� �����Ͽ� Ȱ�� ������ �� �ֵ��� ��
/// </summary>
public class ArrowInteractable : MonoBehaviour
{
    [Header("Arrow Settings")]
    [Tooltip("ȭ���� ������")]
    public int damage = 10;

    [Tooltip("ȭ���� �ı��Ǳ������ �ð�")]
    public float destroyDelay = 10f;

    [Header("Physics")]
    [Tooltip("ȭ���� ����")]
    public float arrowMass = 0.1f;

    [Tooltip("ȭ���� �巡��")]
    public float arrowDrag = 0.1f;

    [Header("Effects")]
    [Tooltip("ȭ���� �¾��� ���� ����Ʈ")]
    public GameObject hitEffectPrefab;

    [Tooltip("ȭ���� �¾��� ���� �Ҹ�")]
    public AudioClip hitSound;

    // ���� ������
    private XRGrabInteractable _grabInteractable;
    private Rigidbody _rigidbody;
    private AudioSource _audioSource;
    private bool _hasHit = false;

    void Start()
    {
        InitializeArrow();
    }

    /// <summary>
    /// ȭ�� �ʱ�ȭ
    /// </summary>
    void InitializeArrow()
    {
        // XR Grab Interactable ����
        _grabInteractable = GetComponent<XRGrabInteractable>();
        if (_grabInteractable == null)
        {
            _grabInteractable = gameObject.AddComponent<XRGrabInteractable>();
        }

        // Rigidbody ����
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            _rigidbody = gameObject.AddComponent<Rigidbody>();
        }

        // Rigidbody �Ӽ� ����
        _rigidbody.mass = arrowMass;
        _rigidbody.drag = arrowDrag;
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;

        // AudioSource ����
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        // �̺�Ʈ ����
        _grabInteractable.selectEntered.AddListener(OnGrabbed);
        _grabInteractable.selectExited.AddListener(OnReleased);
    }

    /// <summary>
    /// ȭ���� ������ �� ȣ��
    /// </summary>
    void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("ȭ���� �������ϴ�!");

        // ȭ���� ������ �� ���� ��Ȱ��ȭ (���Ͽ� �� ������)
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
    }

    /// <summary>
    /// ȭ���� �������� �� ȣ��
    /// </summary>
    void OnReleased(SelectExitEventArgs args)
    {
        Debug.Log("ȭ���� ���������ϴ�!");

        // ȭ���� �������� �� ���� Ȱ��ȭ
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
    }

    /// <summary>
    /// ȭ���� �ٸ� ������Ʈ�� �浹���� �� ȣ��
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        if (_hasHit) return;

        _hasHit = true;

        // �浹 ������ ����Ʈ ����
        if (hitEffectPrefab != null)
        {
            Vector3 hitPoint = collision.contacts[0].point;
            Quaternion hitRotation = Quaternion.LookRotation(collision.contacts[0].normal);
            GameObject hitEffect = Instantiate(hitEffectPrefab, hitPoint, hitRotation);
            Destroy(hitEffect, 3f);
        }

        // ��Ʈ ���� ���
        if (hitSound != null)
        {
            _audioSource.PlayOneShot(hitSound);
        }

        // Ÿ�ٿ� ������ ����
        TargetController target = collision.gameObject.GetComponent<TargetController>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        // ȭ�� �ı�
        Destroy(gameObject, destroyDelay);
    }

    /// <summary>
    /// ȭ���� ȭ�� ������ ������ �� �ڵ� �ı�
    /// </summary>
    void OnBecameInvisible()
    {
        if (!_hasHit)
        {
            Destroy(gameObject, 2f);
        }
    }

    void OnDestroy()
    {
        // �̺�Ʈ ���� ����
        if (_grabInteractable != null)
        {
            _grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            _grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }
}