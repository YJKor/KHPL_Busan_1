using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BowController : MonoBehaviour
{
    [Header("String & Nocking")]
    [SerializeField] private LineRenderer bowStringRenderer; // ������ ǥ���� ���� ������
    [SerializeField] private Transform stringStartPoint;     // ���� ������
    [SerializeField] private Transform stringEndPoint;       // ���� ����
    [SerializeField] private XRSocketInteractor nockSocket;  // ȭ���� ���� ����

    [Header("Arrow")]
    private IXRSelectInteractable nockedArrow = null; // ���� ������ ȭ��
    private bool isArrowNocked = false;               // ȭ���� ���������� ����

    [Header("Shooting")]
    [SerializeField] private float shootingForceMultiplier = 20f; // �߻� �� ���

    private void Awake()
    {
        // XR Socket Interactor�� �̺�Ʈ�� �Լ��� �����մϴ�.
        nockSocket.selectEntered.AddListener(OnArrowNocked);
        nockSocket.selectExited.AddListener(OnArrowRemoved);
    }

    private void OnDestroy()
    {
        // ������Ʈ �ı� �� ������ ������ �����մϴ�.
        nockSocket.selectEntered.RemoveListener(OnArrowNocked);
        nockSocket.selectExited.RemoveListener(OnArrowRemoved);
    }

    // �� ������ ȣ��
    void Update()
    {
        if (isArrowNocked)
        {
            // ȭ���� ������ �ִٸ�, ȭ���� ��� �ִ� ���� ��ġ�� ������ ���ϴ�.
            // nockedArrow.firstInteractorSelecting �� ȭ���� ��� �ִ� ��Ʈ�ѷ�(��)�� �ǹ��մϴ�.
            IXRSelectInteractor hand = nockedArrow.firstInteractorSelecting;
            if (hand != null)
            {
                // ���� �������� �߰����� ���� ��ġ�� ������Ʈ
                UpdateBowString(hand.transform.position);
            }
        }
        else
        {
            // ȭ���� ���ٸ� ������ �⺻ ��ġ�� �ǵ����ϴ�.
            ResetBowString();
        }
    }

    // ȭ���� ���Ͽ� �������� �� ȣ��Ǵ� �Լ�
    private void OnArrowNocked(SelectEnterEventArgs args)
    {
        nockedArrow = args.interactableObject;
        isArrowNocked = true;

        // ȭ���� �߻�� ��(���Ͽ��� �������� ��) Shoot �Լ��� ȣ���ϵ��� �̺�Ʈ�� ����
        nockedArrow.selectExited.AddListener(Shoot);
    }

    // ȭ���� ���Ͽ��� ���������� ��(�߻�ǰų� �׳� ���� ��) ȣ��
    private void OnArrowRemoved(SelectExitEventArgs args)
    {
        // ���� �� �̺�Ʈ ������ ����
        if (args.interactableObject == nockedArrow)
        {
            nockedArrow.selectExited.RemoveListener(Shoot);
            ResetBowString();
            nockedArrow = null;
            isArrowNocked = false;
        }
    }

    // ȭ�� �߻� ����
    private void Shoot(SelectExitEventArgs args)
    {
        // args.interactorObject�� ȭ���� ��� �ִ� ��(��Ʈ�ѷ�)
        // ���� ����մϴ� (������ ��� �Ÿ��� ���)
        float pullDistance = Vector3.Distance(args.interactorObject.transform.position, nockSocket.transform.position);
        float finalForce = pullDistance * shootingForceMultiplier;

        // ȭ���� ���Ͽ��� �и��ϰ� �������� Ȱ��ȭ
        Rigidbody arrowRigidbody = nockedArrow.transform.GetComponent<Rigidbody>();
        arrowRigidbody.isKinematic = false;
        arrowRigidbody.useGravity = true;

        // ȭ���� �� �������� ���� ���մϴ�.
        arrowRigidbody.AddForce(nockedArrow.transform.forward * finalForce, ForceMode.Impulse);
    }

    // ����(Line Renderer)�� �⺻ ��ġ�� ����
    private void ResetBowString()
    {
        bowStringRenderer.positionCount = 2;
        bowStringRenderer.SetPosition(0, stringStartPoint.position);
        bowStringRenderer.SetPosition(1, stringEndPoint.position);
    }

    // ������ ��� ��ġ�� ������Ʈ
    private void UpdateBowString(Vector3 pullPosition)
    {
        bowStringRenderer.positionCount = 3;
        bowStringRenderer.SetPosition(0, stringStartPoint.position);
        bowStringRenderer.SetPosition(1, pullPosition);
        bowStringRenderer.SetPosition(2, stringEndPoint.position);
    }
}
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.XR.Interaction.Toolkit;

///// <summary>
///// VR Ȱ ��Ʈ�ѷ� - XR Interaction Toolkit�� ����
///// Ȱ ���� ����, ȭ�� �߻�, ���� �ùķ��̼��� ���
///// </summary>
//public class BowController : MonoBehaviour
//{
//    [Header("Bow References")]
//    [Tooltip("Ȱ ������ ������ (Ȱ ��ü�� ����� �κ�)")]
//    public Transform stringAttachPoint;

//    [Tooltip("ȭ���� ���۵Ǵ� ��ġ")]
//    public Transform arrowStartPoint;

//    [Tooltip("ȭ�� ������")]
//    public GameObject arrowPrefab;

//    [Header("Bow Physics")]
//    [Tooltip("Ȱ ������ �ִ� ��� �Ÿ�")]
//    public float maxPullDistance = 0.5f;

//    [Tooltip("Ȱ ������ ź�� ��� (�������� �� ���� �߻�)")]
//    public float bowTension = 1000f;

//    [Tooltip("ȭ�� �߻� �ӵ� ���")]
//    public float arrowSpeedMultiplier = 1.5f;

//    [Header("Visual Feedback")]
//    [Tooltip("Ȱ ������ �ð������� ǥ���ϴ� ���� ������")]
//    public LineRenderer bowString;

//    [Tooltip("Ȱ ������ ����")]
//    public Color stringColor = Color.white;

//    [Header("Audio")]
//    [Tooltip("Ȱ ���� ���� �Ҹ�")]
//    public AudioClip pullSound;

//    [Tooltip("ȭ�� �߻� �Ҹ�")]
//    public AudioClip releaseSound;

//    [Tooltip("ȭ�� ���� �Ҹ�")]
//    public AudioClip nockSound;

//    // ���� ������
//    private GameObject _currentArrow;
//    private bool _isStringPulled = false;
//    private Transform _pullingHand;
//    private float _currentPullDistance = 0f;
//    private Vector3 _originalStringPosition;
//    private AudioSource _audioSource;
//    private XRGrabInteractable _bowGrabInteractable;

//    // �̺�Ʈ
//    public System.Action<float> OnPullStrengthChanged;
//    public System.Action OnArrowReleased;

//    void Start()
//    {
//        InitializeBow();
//    }

//    void Update()
//    {
//        UpdateBowString();
//        UpdatePullPhysics();
//    }

//    /// <summary>
//    /// Ȱ �ʱ�ȭ
//    /// </summary>
//    void InitializeBow()
//    {
//        // AudioSource ����
//        _audioSource = GetComponent<AudioSource>();
//        if (_audioSource == null)
//        {
//            _audioSource = gameObject.AddComponent<AudioSource>();
//        }

//        // XR Grab Interactable ����
//        _bowGrabInteractable = GetComponent<XRGrabInteractable>();
//        if (_bowGrabInteractable == null)
//        {
//            _bowGrabInteractable = gameObject.AddComponent<XRGrabInteractable>();
//        }

//        // Ȱ ���� ���� ������ ����
//        SetupBowString();

//        // �ʱ� Ȱ ���� ��ġ ����
//        _originalStringPosition = stringAttachPoint.position;
//    }

//    /// <summary>
//    /// Ȱ ���� ���� ������ ����
//    /// </summary>
//    void SetupBowString()
//    {
//        if (bowString == null)
//        {
//            bowString = gameObject.AddComponent<LineRenderer>();
//        }

//        bowString.material = new Material(Shader.Find("Sprites/Default"));
//        //bowString.color = stringColor;
//        bowString.startWidth = 0.01f;
//        bowString.endWidth = 0.01f;
//        bowString.positionCount = 2;
//        bowString.useWorldSpace = true;
//    }

//    /// <summary>
//    /// Ȱ ���� �ð��� ������Ʈ
//    /// </summary>
//    void UpdateBowString()
//    {
//        if (bowString != null)
//        {
//            Vector3 startPos = _originalStringPosition;
//            Vector3 endPos = _isStringPulled ? _pullingHand.position : _originalStringPosition;

//            bowString.SetPosition(0, startPos);
//            bowString.SetPosition(1, endPos);
//        }
//    }

//    /// <summary>
//    /// Ȱ ���� ���� ���� ������Ʈ
//    /// </summary>
//    void UpdatePullPhysics()
//    {
//        if (_isStringPulled && _pullingHand != null)
//        {
//            // ���� ��� �Ÿ� ���
//            _currentPullDistance = Vector3.Distance(_originalStringPosition, _pullingHand.position);
//            _currentPullDistance = Mathf.Clamp(_currentPullDistance, 0f, maxPullDistance);

//            // ��� ���� �̺�Ʈ ȣ�� (0-1 ����)
//            float pullStrength = _currentPullDistance / maxPullDistance;
//            OnPullStrengthChanged?.Invoke(pullStrength);

//            // Ȱ ���� ��ġ ������Ʈ
//            stringAttachPoint.position = Vector3.Lerp(_originalStringPosition, _pullingHand.position, 0.5f);
//        }
//    }

//    /// <summary>
//    /// Ȱ ���� ���� ����
//    /// </summary>
//    /// <param name="hand">���� ���� Transform</param>
//    public void StartPull(Transform hand)
//    {
//        if (!_isStringPulled)
//        {
//            _isStringPulled = true;
//            _pullingHand = hand;
//            _currentPullDistance = 0f;

//            // ȭ�� ����
//            CreateArrow();

//            // ���� �Ҹ� ���
//            if (pullSound != null)
//            {
//                _audioSource.PlayOneShot(pullSound);
//            }
//        }
//    }

//    /// <summary>
//    /// ȭ�� ����
//    /// </summary>
//    void CreateArrow()
//    {
//        if (arrowPrefab != null && _currentArrow == null)
//        {
//            _currentArrow = Instantiate(arrowPrefab, arrowStartPoint.position, arrowStartPoint.rotation, arrowStartPoint);

//            // ȭ���� Rigidbody�� ��Ȱ��ȭ (�߻� ������)
//            Rigidbody arrowRb = _currentArrow.GetComponent<Rigidbody>();
//            if (arrowRb != null)
//            {
//                arrowRb.isKinematic = true;
//            }

//            // ���� �Ҹ� ���
//            if (nockSound != null)
//            {
//                _audioSource.PlayOneShot(nockSound);
//            }
//        }
//    }

//    /// <summary>
//    /// Ȱ ���� ���� (ȭ�� �߻�)
//    /// </summary>
//    public void ReleasePull()
//    {
//        if (_isStringPulled && _currentArrow != null)
//        {
//            // ȭ�� �߻�
//            FireArrow();

//            // Ȱ ���� ����ġ
//            ResetBowString();

//            // �߻� �Ҹ� ���
//            if (releaseSound != null)
//            {
//                _audioSource.PlayOneShot(releaseSound);
//            }

//            // �̺�Ʈ ȣ��
//            OnArrowReleased?.Invoke();
//        }

//        _isStringPulled = false;
//        _pullingHand = null;
//        _currentArrow = null;
//    }

//    /// <summary>
//    /// ȭ�� �߻�
//    /// </summary>
//    void FireArrow()
//    {
//        // ȭ���� �θ𿡼� �и�
//        _currentArrow.transform.parent = null;

//        // Rigidbody Ȱ��ȭ
//        Rigidbody arrowRb = _currentArrow.GetComponent<Rigidbody>();
//        if (arrowRb != null)
//        {
//            arrowRb.isKinematic = false;

//            // �߻� ���� ��� (Ȱ ���� ����)
//            Vector3 fireDirection = (arrowStartPoint.position - stringAttachPoint.position).normalized;

//            // �߻� �� ��� (��� �Ÿ��� ���)
//            float pullStrength = _currentPullDistance / maxPullDistance;
//            float fireForce = bowTension * pullStrength * arrowSpeedMultiplier;

//            // ȭ�쿡 �� ����
//            arrowRb.AddForce(fireDirection * fireForce, ForceMode.Impulse);

//            // ȭ�� ȸ�� �߰� (�� �������� ����)
//            arrowRb.AddTorque(arrowRb.transform.right * 10f, ForceMode.Impulse);
//        }
//    }

//    /// <summary>
//    /// Ȱ ���� ����ġ
//    /// </summary>
//    void ResetBowString()
//    {
//        stringAttachPoint.position = _originalStringPosition;
//        _currentPullDistance = 0f;
//        OnPullStrengthChanged?.Invoke(0f);
//    }

//    /// <summary>
//    /// ���� ��� ���� ��ȯ (0-1)
//    /// </summary>
//    public float GetPullStrength()
//    {
//        return _currentPullDistance / maxPullDistance;
//    }

//    /// <summary>
//    /// Ȱ�� �����ִ��� Ȯ��
//    /// </summary>
//    public bool IsGrabbed()
//    {
//        return _bowGrabInteractable != null && _bowGrabInteractable.isSelected;
//    }
//}