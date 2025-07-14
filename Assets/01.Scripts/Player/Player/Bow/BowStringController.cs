//using UnityEngine;
//using UnityEngine.XR.Interaction.Toolkit;

///// <summary>
///// VR Ȱ ���� ��Ʈ�ѷ�
///// XR ��Ʈ�ѷ��� Ȱ ������ �����Ͽ� ����/���� ������ ó��
///// </summary>
//public class BowStringController : MonoBehaviour
//{
//    [Header("Bow Reference")]
//    [Tooltip("������ Ȱ ��Ʈ�ѷ�")]
//    public BowController bowController;

//    [Header("Interaction Settings")]
//    [Tooltip("Ȱ ������ ��ȣ�ۿ��� �� �ִ� �Ÿ�")]
//    public float interactionDistance = 0.1f;

//    [Tooltip("Ȱ ������ ���� �� �ִ� ����")]
//    public Collider stringCollider;

//    [Header("Haptic Feedback")]
//    [Tooltip("Ȱ ���� ��� �� ��ƽ �ǵ�� ����")]
//    public float pullHapticStrength = 0.5f;

//    [Tooltip("Ȱ ���� ���� �� ��ƽ �ǵ�� ����")]
//    public float releaseHapticStrength = 0.8f;

//    // ���� ������
//    private XRGrabInteractable _stringGrabInteractable;
//    private XRDirectInteractor _currentInteractor;
//    private bool _isBeingPulled = false;
//    private Vector3 _originalPosition;

//    void Start()
//    {
//        InitializeStringController();
//    }

//    /// <summary>
//    /// Ȱ ���� ��Ʈ�ѷ� �ʱ�ȭ
//    /// </summary>
//    void InitializeStringController()
//    {
//        // ���� ��ġ ����
//        _originalPosition = transform.position;

//        // XR Grab Interactable ����
//        _stringGrabInteractable = GetComponent<XRGrabInteractable>();
//        if (_stringGrabInteractable == null)
//        {
//            _stringGrabInteractable = gameObject.AddComponent<XRGrabInteractable>();
//        }

//        // �̺�Ʈ ����
//        _stringGrabInteractable.selectEntered.AddListener(OnStringGrabbed);
//        _stringGrabInteractable.selectExited.AddListener(OnStringReleased);
//        _stringGrabInteractable.hoverEntered.AddListener(OnStringHoverEnter);
//        _stringGrabInteractable.hoverExited.AddListener(OnStringHoverExit);

//        // Ȱ ������ Ȱ�� �Բ� �������� �ϹǷ� �θ� ����
//        if (bowController != null)
//        {
//            transform.SetParent(bowController.transform);
//        }
//    }

//    /// <summary>
//    /// Ȱ ������ ������ �� ȣ��
//    /// </summary>
//    /// <param name="args">���� �̺�Ʈ ����</param>
//    void OnStringGrabbed(SelectEnterEventArgs args)
//    {
//        _currentInteractor = args.interactorObject as XRDirectInteractor;
//        if (_currentInteractor != null && bowController != null)
//        {
//            _isBeingPulled = true;

//            // Ȱ ���� ���� ����
//            bowController.StartPull(_currentInteractor.transform);

//            // ��ƽ �ǵ��
//            SendHapticFeedback(pullHapticStrength);
//        }
//    }

//    /// <summary>
//    /// Ȱ ������ �������� �� ȣ��
//    /// </summary>
//    /// <param name="args">���� ���� �̺�Ʈ ����</param>
//    void OnStringReleased(SelectExitEventArgs args)
//    {
//        if (_isBeingPulled && bowController != null)
//        {
//            _isBeingPulled = false;

//            // Ȱ ���� ���� (ȭ�� �߻�)
//            bowController.ReleasePull();

//            // ��ƽ �ǵ��
//            SendHapticFeedback(releaseHapticStrength);
//        }

//        _currentInteractor = null;
//    }

//    /// <summary>
//    /// Ȱ ������ ȣ�� ����
//    /// </summary>
//    /// <param name="args">ȣ�� �̺�Ʈ ����</param>
//    void OnStringHoverEnter(HoverEnterEventArgs args)
//    {
//        // ȣ�� �� �ð��� �ǵ�� (���û���)
//        Renderer renderer = GetComponent<Renderer>();
//        if (renderer != null)
//        {
//            renderer.material.color = Color.yellow;
//        }
//    }

//    /// <summary>
//    /// Ȱ �������� ȣ�� ����
//    /// </summary>
//    /// <param name="args">ȣ�� ���� �̺�Ʈ ����</param>
//    void OnStringHoverExit(HoverExitEventArgs args)
//    {
//        // ȣ�� ���� �� ���� �������� ����
//        Renderer renderer = GetComponent<Renderer>();
//        if (renderer != null)
//        {
//            renderer.material.color = Color.white;
//        }
//    }

//    /// <summary>
//    /// ��ƽ �ǵ�� ����
//    /// </summary>
//    /// <param name="strength">�ǵ�� ���� (0-1)</param>
//    void SendHapticFeedback(float strength)
//    {
//        if (_currentInteractor != null)
//        {
//            XRBaseController controller = _currentInteractor.GetComponent<XRBaseController>();
//            if (controller != null)
//            {
//                controller.SendHapticImpulse(strength, 0.1f);
//            }
//        }
//    }

//    void Update()
//    {
//        // Ȱ ������ ������� ���� �� ��ġ ������Ʈ
//        if (_isBeingPulled && _currentInteractor != null)
//        {
//            // ��Ʈ�ѷ� ��ġ�� Ȱ ���� �̵�
//            transform.position = _currentInteractor.transform.position;

//            // Ȱ ������ �ʹ� �ָ� ������� �ʵ��� ����
//            float distanceFromOriginal = Vector3.Distance(_originalPosition, transform.position);
//            if (distanceFromOriginal > bowController.maxPullDistance)
//            {
//                Vector3 direction = (transform.position - _originalPosition).normalized;
//                transform.position = _originalPosition + direction * bowController.maxPullDistance;
//            }
//        }
//        else if (!_isBeingPulled)
//        {
//            // Ȱ ������ �������� �� ���� ��ġ�� ����
//            transform.position = _originalPosition;
//        }
//    }

//    /// <summary>
//    /// Ȱ ������ ������� �ִ��� Ȯ��
//    /// </summary>
//    public bool IsBeingPulled()
//    {
//        return _isBeingPulled;
//    }

//    /// <summary>
//    /// ���� ��� �Ÿ� ��ȯ
//    /// </summary>
//    public float GetCurrentPullDistance()
//    {
//        if (_isBeingPulled)
//        {
//            return Vector3.Distance(_originalPosition, transform.position);
//        }
//        return 0f;
//    }

//    void OnDestroy()
//    {
//        // �̺�Ʈ ���� ����
//        if (_stringGrabInteractable != null)
//        {
//            _stringGrabInteractable.selectEntered.RemoveListener(OnStringGrabbed);
//            _stringGrabInteractable.selectExited.RemoveListener(OnStringReleased);
//            _stringGrabInteractable.hoverEntered.RemoveListener(OnStringHoverEnter);
//            _stringGrabInteractable.hoverExited.RemoveListener(OnStringHoverExit);
//        }
//    }
//}