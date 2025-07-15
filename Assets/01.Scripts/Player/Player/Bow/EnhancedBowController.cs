using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

/// <summary>
/// The Lab VR ��Ÿ���� ���� Ȱ ��Ʈ�ѷ�
/// �����տ� ȭ�� �ڵ� ����, �ڿ������� ���� ����, ��Ȯ�� �߻� �ý���
/// </summary>
public class EnhancedBowController : MonoBehaviour
{
    [Header("Bow References")]
    [Tooltip("Ȱ ������ ǥ���� ���� ������")]
    [SerializeField] private LineRenderer bowStringRenderer;

    [Tooltip("���� ������")]
    [SerializeField] private Transform stringStartPoint;

    [Tooltip("���� ����")]
    [SerializeField] private Transform stringEndPoint;

    [Tooltip("ȭ���� ������ ����")]
    [SerializeField] private XRSocketInteractor nockSocket;

    [Tooltip("���� ��� ���� ����")]
    [SerializeField] private Transform stringPullArea;

    [Header("Arrow System")]
    [Tooltip("ȭ�� ������")]
    [SerializeField] private GameObject arrowPrefab;

    [Tooltip("������ ȭ�� ���� ��ġ")]
    [SerializeField] private Transform rightHandArrowSpawn;

    [Tooltip("�ڵ� ȭ�� ���� ���� (��)")]
    [SerializeField] private float arrowSpawnInterval = 1.5f;

    [Tooltip("�ִ� ���� ȭ�� ����")]
    [SerializeField] private int maxArrows = 10;

    [Header("Bow Physics")]
    [Tooltip("�߻� �� ���")]
    [SerializeField] private float shootingForceMultiplier = 25f;

    [Tooltip("�ִ� ��� �Ÿ�")]
    [SerializeField] private float maxPullDistance = 0.6f;

    [Tooltip("���� ź�� ���")]
    [SerializeField] private float stringTension = 1.2f;

    [Tooltip("ȭ�� ȸ����")]
    [SerializeField] private float arrowSpinForce = 15f;

    [Header("Visual & Audio")]
    [Tooltip("���� ��� ����")]
    [SerializeField] private AudioClip pullSound;

    [Tooltip("ȭ�� �߻� ����")]
    [SerializeField] private AudioClip releaseSound;

    [Tooltip("ȭ�� ���� ����")]
    [SerializeField] private AudioClip nockSound;

    [Tooltip("���� ����")]
    [SerializeField] private Color stringColor = Color.white;

    [Tooltip("���� �β�")]
    [SerializeField] private float stringWidth = 0.005f;

    [Header("Debug")]
    [Tooltip("����� �α� Ȱ��ȭ")]
    [SerializeField] private bool enableDebugLogs = true;

    // ���� ������
    private IXRSelectInteractable nockedArrow = null;
    private bool isArrowNocked = false;
    private bool isStringPulled = false;
    private Transform pullingHand = null;
    private float currentPullDistance = 0f;
    private Vector3 originalStringPosition;
    private AudioSource audioSource;
    private int currentArrowCount = 0;
    private Coroutine arrowSpawnCoroutine;
    private XRDirectInteractor rightHandInteractor;

    // �̺�Ʈ
    public System.Action<float> OnPullStrengthChanged;
    public System.Action OnArrowReleased;
    public System.Action<int> OnArrowCountChanged;

    void Start()
    {
        InitializeBow();
    }

    void Update()
    {
        UpdateBowString();
        UpdatePullPhysics();
    }

    /// <summary>
    /// Ȱ �ʱ�ȭ
    /// </summary>
    void InitializeBow()
    {
        // AudioSource ����
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ������ ��Ʈ�ѷ� ã��
        FindRightHandController();

        // ���� �ʱ�ȭ
        SetupBowString();

        // ���� �̺�Ʈ ����
        if (nockSocket != null)
        {
            nockSocket.selectEntered.AddListener(OnArrowNocked);
            nockSocket.selectExited.AddListener(OnArrowRemoved);
        }

        // ���� ��� ���� ����
        SetupStringPullDetection();

        // �ڵ� ȭ�� ���� ����
        StartArrowSpawning();

        if (enableDebugLogs)
            Debug.Log("���� Ȱ ��Ʈ�ѷ��� �ʱ�ȭ�Ǿ����ϴ�.");
    }

    /// <summary>
    /// ������ ��Ʈ�ѷ� ã��
    /// </summary>
    void FindRightHandController()
    {
        XRDirectInteractor[] interactors = FindObjectsOfType<XRDirectInteractor>();
        foreach (var interactor in interactors)
        {
            if (interactor.name.ToLower().Contains("right") ||
                interactor.name.ToLower().Contains("r_") ||
                interactor.name.ToLower().Contains("r "))
            {
                rightHandInteractor = interactor;
                break;
            }
        }

        if (rightHandInteractor == null && interactors.Length > 0)
        {
            rightHandInteractor = interactors[0];
            Debug.LogWarning("������ ��Ʈ�ѷ��� ��Ȯ�� ã�� ���߽��ϴ�. ù ��° ��Ʈ�ѷ��� ����մϴ�.");
        }

        // ������ ȭ�� ���� ��ġ ����
        if (rightHandArrowSpawn == null && rightHandInteractor != null)
        {
            rightHandArrowSpawn = rightHandInteractor.transform;
        }
    }

    /// <summary>
    /// ���� �ð��� ����
    /// </summary>
    void SetupBowString()
    {
        if (bowStringRenderer == null)
        {
            bowStringRenderer = gameObject.AddComponent<LineRenderer>();
        }

        bowStringRenderer.material = new Material(Shader.Find("Sprites/Default"));
        bowStringRenderer.startColor = stringColor;
        bowStringRenderer.endColor = stringColor;
        bowStringRenderer.startWidth = stringWidth;
        bowStringRenderer.endWidth = stringWidth;
        bowStringRenderer.positionCount = 2;
        bowStringRenderer.useWorldSpace = true;

        // ���� ���� ��ġ ����
        if (stringStartPoint != null && stringEndPoint != null)
        {
            originalStringPosition = (stringStartPoint.position + stringEndPoint.position) * 0.5f;
            ResetBowString();
        }
    }

    /// <summary>
    /// ���� ��� ���� ����
    /// </summary>
    void SetupStringPullDetection()
    {
        if (stringPullArea == null)
        {
            // ���� ��� ���� ���� ����
            GameObject pullArea = new GameObject("StringPullArea");
            pullArea.transform.SetParent(transform);
            pullArea.transform.position = originalStringPosition;

            SphereCollider pullCollider = pullArea.AddComponent<SphereCollider>();
            pullCollider.isTrigger = true;
            pullCollider.radius = 0.1f;

            StringPullDetector detector = pullArea.AddComponent<StringPullDetector>();
            detector.bowController = this;

            stringPullArea = pullArea.transform;
        }
    }

    /// <summary>
    /// �ڵ� ȭ�� ���� ����
    /// </summary>
    void StartArrowSpawning()
    {
        if (arrowSpawnCoroutine != null)
        {
            StopCoroutine(arrowSpawnCoroutine);
        }
        arrowSpawnCoroutine = StartCoroutine(AutoArrowSpawn());
    }

    /// <summary>
    /// �ڵ� ȭ�� ���� �ڷ�ƾ
    /// </summary>
    IEnumerator AutoArrowSpawn()
    {
        while (true)
        {
            if (currentArrowCount < maxArrows)
            {
                SpawnArrowInHand();
            }
            yield return new WaitForSeconds(arrowSpawnInterval);
        }
    }

    /// <summary>
    /// �����տ� ȭ�� ����
    /// </summary>
    void SpawnArrowInHand()
    {
        if (arrowPrefab == null || rightHandArrowSpawn == null) return;

        // ȭ�� ���� ��ġ (������ ����)
        Vector3 spawnPos = rightHandArrowSpawn.position + rightHandArrowSpawn.forward * 0.15f;
        GameObject newArrow = Instantiate(arrowPrefab, spawnPos, rightHandArrowSpawn.rotation);

        // ȭ�� ������Ʈ ����
        SetupArrowComponents(newArrow);

        currentArrowCount++;
        OnArrowCountChanged?.Invoke(currentArrowCount);

        if (enableDebugLogs)
            Debug.Log($"�����տ� ȭ���� �����Ǿ����ϴ�. ���� ����: {currentArrowCount}");
    }

    /// <summary>
    /// ȭ�� ������Ʈ ����
    /// </summary>
    void SetupArrowComponents(GameObject arrow)
    {
        // XR Grab Interactable
        XRGrabInteractable grabInteractable = arrow.GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = arrow.AddComponent<XRGrabInteractable>();
        }

        // Rigidbody
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = arrow.AddComponent<Rigidbody>();
        }
        rb.mass = 0.1f;
        rb.drag = 0.1f;

        // ArrowController
        ArrowController arrowController = arrow.GetComponent<ArrowController>();
        if (arrowController == null)
        {
            arrowController = arrow.AddComponent<ArrowController>();
        }

        // ArrowInteractable
        ArrowInteractable arrowInteractable = arrow.GetComponent<ArrowInteractable>();
        if (arrowInteractable == null)
        {
            arrowInteractable = arrow.AddComponent<ArrowInteractable>();
        }

        // ȭ�� �ı� �� ī��Ʈ ����
        //arrowInteractable.OnArrowDestroyed += () => {
        //    currentArrowCount = Mathf.Max(0, currentArrowCount - 1);
        //    OnArrowCountChanged?.Invoke(currentArrowCount);
        //};

        // �ʱ� ���� ��Ȱ��ȭ (�ڿ������� ���)
        rb.isKinematic = true;
        rb.useGravity = false;
        StartCoroutine(EnableArrowPhysics(rb, 0.3f));
    }

    /// <summary>
    /// ȭ�� ���� Ȱ��ȭ ����
    /// </summary>
    IEnumerator EnableArrowPhysics(Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    /// <summary>
    /// ���� �ð��� ������Ʈ
    /// </summary>
    void UpdateBowString()
    {
        if (bowStringRenderer == null) return;

        Vector3 startPos = stringStartPoint.position;
        Vector3 endPos = stringEndPoint.position;

        if (isStringPulled && pullingHand != null)
        {
            // ������ ����� ����
            Vector3 pullPos = pullingHand.position;
            Vector3 direction = (pullPos - originalStringPosition).normalized;
            float distance = Vector3.Distance(originalStringPosition, pullPos);
            float clampedDistance = Mathf.Clamp(distance, 0f, maxPullDistance);
            Vector3 clampedPullPos = originalStringPosition + direction * clampedDistance;

            bowStringRenderer.positionCount = 3;
            bowStringRenderer.SetPosition(0, startPos);
            bowStringRenderer.SetPosition(1, clampedPullPos);
            bowStringRenderer.SetPosition(2, endPos);
        }
        else
        {
            // ������ ������� ���� ����
            bowStringRenderer.positionCount = 2;
            bowStringRenderer.SetPosition(0, startPos);
            bowStringRenderer.SetPosition(1, endPos);
        }
    }

    /// <summary>
    /// ���� ��� ���� ������Ʈ
    /// </summary>
    void UpdatePullPhysics()
    {
        if (isStringPulled && pullingHand != null)
        {
            currentPullDistance = Vector3.Distance(originalStringPosition, pullingHand.position);
            currentPullDistance = Mathf.Clamp(currentPullDistance, 0f, maxPullDistance);

            float pullStrength = currentPullDistance / maxPullDistance;
            OnPullStrengthChanged?.Invoke(pullStrength);
        }
    }

    /// <summary>
    /// ȭ���� ���Ͽ� ������ ��
    /// </summary>
    private void OnArrowNocked(SelectEnterEventArgs args)
    {
        nockedArrow = args.interactableObject;
        isArrowNocked = true;

        if (nockSound != null)
        {
            audioSource.PlayOneShot(nockSound);
        }

        if (enableDebugLogs)
            Debug.Log("ȭ���� Ȱ�� �����Ǿ����ϴ�.");
    }

    /// <summary>
    /// ȭ���� ���Ͽ��� ���ŵ� ��
    /// </summary>
    private void OnArrowRemoved(SelectExitEventArgs args)
    {
        if (args.interactableObject == nockedArrow)
        {
            ResetBowString();
            nockedArrow = null;
            isArrowNocked = false;
            isStringPulled = false;
            pullingHand = null;
            currentPullDistance = 0f;
        }
    }

    /// <summary>
    /// ���� ��� ���� (StringPullDetector���� ȣ��)
    /// </summary>
    public void StartStringPull(Transform hand)
    {
        if (!isStringPulled && isArrowNocked)
        {
            isStringPulled = true;
            pullingHand = hand;
            currentPullDistance = 0f;

            if (pullSound != null)
            {
                audioSource.PlayOneShot(pullSound);
            }

            if (enableDebugLogs)
                Debug.Log("������ ���� �����߽��ϴ�.");
        }
    }

    /// <summary>
    /// ���� ��� ���� (ȭ�� �߻�)
    /// </summary>
    public void ReleaseStringPull()
    {
        if (isStringPulled && isArrowNocked && nockedArrow != null)
        {
            FireArrow();
            ResetBowString();

            if (releaseSound != null)
            {
                audioSource.PlayOneShot(releaseSound);
            }

            OnArrowReleased?.Invoke();

            if (enableDebugLogs)
                Debug.Log("ȭ���� �߻��߽��ϴ�.");
        }

        isStringPulled = false;
        pullingHand = null;
        currentPullDistance = 0f;
    }

    /// <summary>
    /// ȭ�� �߻�
    /// </summary>
    void FireArrow()
    {
        Rigidbody arrowRb = nockedArrow.transform.GetComponent<Rigidbody>();
        if (arrowRb != null)
        {
            arrowRb.isKinematic = false;
            arrowRb.useGravity = true;

            // �߻� ���� ��� (���� ����)
            Vector3 fireDirection = (stringEndPoint.position - stringStartPoint.position).normalized;

            // �߻� �� ��� (��� �Ÿ��� ���)
            float pullStrength = currentPullDistance / maxPullDistance;
            float fireForce = shootingForceMultiplier * pullStrength * stringTension;

            // ȭ�쿡 �� ����
            arrowRb.AddForce(fireDirection * fireForce, ForceMode.Impulse);

            // ȭ�� ȸ�� �߰� (�� �ڿ������� ����)
            arrowRb.AddTorque(arrowRb.transform.right * arrowSpinForce, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// ���� �ʱ� ��ġ�� ����
    /// </summary>
    void ResetBowString()
    {
        if (bowStringRenderer != null)
        {
            bowStringRenderer.positionCount = 2;
            bowStringRenderer.SetPosition(0, stringStartPoint.position);
            bowStringRenderer.SetPosition(1, stringEndPoint.position);
        }
    }

    /// <summary>
    /// ���� ��� ���� ��ȯ (0-1)
    /// </summary>
    public float GetPullStrength()
    {
        return currentPullDistance / maxPullDistance;
    }

    /// <summary>
    /// ȭ���� �����Ǿ����� Ȯ��
    /// </summary>
    public bool IsArrowNocked()
    {
        return isArrowNocked;
    }

    /// <summary>
    /// ������ ������� �ִ��� Ȯ��
    /// </summary>
    public bool IsStringPulled()
    {
        return isStringPulled;
    }

    /// <summary>
    /// ���� ȭ�� ���� ��ȯ
    /// </summary>
    public int GetCurrentArrowCount()
    {
        return currentArrowCount;
    }

    void OnDestroy()
    {
        if (arrowSpawnCoroutine != null)
        {
            StopCoroutine(arrowSpawnCoroutine);
        }

        if (nockSocket != null)
        {
            nockSocket.selectEntered.RemoveListener(OnArrowNocked);
            nockSocket.selectExited.RemoveListener(OnArrowRemoved);
        }
    }
}