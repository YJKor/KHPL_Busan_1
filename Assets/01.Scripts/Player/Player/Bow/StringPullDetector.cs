using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// ���� ����� �����ϴ� ���� �ý���
/// The Lab VR ��Ÿ���� �ڿ������� ���� ��� ����
/// </summary>
public class StringPullDetector : MonoBehaviour
{
    [Header("References")]
    [Tooltip("���� Ȱ ��Ʈ�ѷ�")]
    public EnhancedBowController bowController;

    [Tooltip("���� Ȱ ��Ʈ�ѷ� (ȣȯ����)")]
    public BowController legacyBowController;

    [Header("Detection Settings")]
    [Tooltip("���� ��� ���� �Ÿ�")]
    public float detectionRadius = 0.15f;

    [Tooltip("���� ��� ���� �Ÿ�")]
    public float releaseDistance = 0.25f;

    [Tooltip("����� �ð�ȭ Ȱ��ȭ")]
    public bool enableDebugVisualization = true;

    // ���� ������
    private bool isPulling = false;
    private Transform currentHand = null;
    private float currentDistance = 0f;
    private SphereCollider detectionCollider;

    void Start()
    {
        SetupDetectionArea();
    }

    void Update()
    {
        if (isPulling && currentHand != null)
        {
            UpdatePullDistance();
        }
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    void SetupDetectionArea()
    {
        detectionCollider = GetComponent<SphereCollider>();
        if (detectionCollider == null)
        {
            detectionCollider = gameObject.AddComponent<SphereCollider>();
        }

        detectionCollider.isTrigger = true;
        detectionCollider.radius = detectionRadius;

        // ����� �ð�ȭ
        if (enableDebugVisualization)
        {
            CreateDebugVisualization();
        }
    }

    /// <summary>
    /// ����� �ð�ȭ ����
    /// </summary>
    void CreateDebugVisualization()
    {
        GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        debugSphere.name = "StringPullDebug";
        debugSphere.transform.SetParent(transform);
        debugSphere.transform.localPosition = Vector3.zero;
        debugSphere.transform.localScale = Vector3.one * detectionRadius * 2f;

        // ������ ���� ����
        Renderer renderer = debugSphere.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material debugMaterial = new Material(Shader.Find("Standard"));
            debugMaterial.color = new Color(1f, 0f, 0f, 0.3f);
            debugMaterial.SetFloat("_Mode", 3); // Transparent mode
            debugMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            debugMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            debugMaterial.SetInt("_ZWrite", 0);
            debugMaterial.DisableKeyword("_ALPHATEST_ON");
            debugMaterial.EnableKeyword("_ALPHABLEND_ON");
            debugMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            debugMaterial.renderQueue = 3000;
            renderer.material = debugMaterial;
        }

        // �ݶ��̴� ���� (Ʈ���Ÿ� ���)
        DestroyImmediate(debugSphere.GetComponent<Collider>());
    }

    /// <summary>
    /// ��� �Ÿ� ������Ʈ
    /// </summary>
    void UpdatePullDistance()
    {
        if (currentHand == null) return;

        currentDistance = Vector3.Distance(transform.position, currentHand.position);

        // ���� ��� ���� �Ÿ��� ����� ����
        if (currentDistance > releaseDistance)
        {
            ReleasePull();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // XR ��Ʈ�ѷ��� �� ����
        if (IsHandController(other) && !isPulling)
        {
            StartPull(other.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // ���� ���� ������ ����� ���� ��� ����
        if (IsHandController(other) && isPulling && other.transform == currentHand)
        {
            ReleasePull();
        }
    }

    /// <summary>
    /// �� ��Ʈ�ѷ����� Ȯ��
    /// </summary>
    bool IsHandController(Collider other)
    {
        // XR ��Ʈ�ѷ� ���� ������Ʈ Ȯ��
        if (other.GetComponent<XRDirectInteractor>() != null ||
            other.GetComponent<XRRayInteractor>() != null ||
            other.GetComponent<XRGrabInteractable>() != null)
        {
            return true;
        }

        // �±׷� Ȯ��
        if (other.CompareTag("Player") ||
            other.CompareTag("Hand") ||
            other.CompareTag("Controller"))
        {
            return true;
        }

        // �̸����� Ȯ��
        string objName = other.name.ToLower();
        if (objName.Contains("hand") ||
            objName.Contains("controller") ||
            objName.Contains("interactor"))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// ���� ��� ����
    /// </summary>
    void StartPull(Transform hand)
    {
        if (isPulling) return;

        isPulling = true;
        currentHand = hand;
        currentDistance = 0f;

        // ���� Ȱ ��Ʈ�ѷ� ���
        if (bowController != null)
        {
            bowController.StartStringPull(hand);
        }
        // ���� Ȱ ��Ʈ�ѷ� ��� (ȣȯ��)
        else if (legacyBowController != null)
        {
            // ���� �޼��尡 �ִٸ� ȣ��
            var method = legacyBowController.GetType().GetMethod("StartPull");
            if (method != null)
            {
                method.Invoke(legacyBowController, new object[] { hand });
            }
        }

        Debug.Log("���� ��� ������");
    }

    /// <summary>
    /// ���� ��� ����
    /// </summary>
    void ReleasePull()
    {
        if (!isPulling) return;

        isPulling = false;
        currentHand = null;
        currentDistance = 0f;

        // ���� Ȱ ��Ʈ�ѷ� ���
        if (bowController != null)
        {
            bowController.ReleaseStringPull();
        }
        // ���� Ȱ ��Ʈ�ѷ� ��� (ȣȯ��)
        else if (legacyBowController != null)
        {
            // ���� �޼��尡 �ִٸ� ȣ��
            var method = legacyBowController.GetType().GetMethod("ReleasePull");
            if (method != null)
            {
                method.Invoke(legacyBowController, null);
            }
        }

        Debug.Log("���� ��� ������");
    }

    /// <summary>
    /// ���� ��� ���� ��ȯ
    /// </summary>
    public bool IsPulling()
    {
        return isPulling;
    }

    /// <summary>
    /// ���� ��� �Ÿ� ��ȯ
    /// </summary>
    public float GetCurrentPullDistance()
    {
        return currentDistance;
    }

    void OnDrawGizmosSelected()
    {
        // ���� ���� �ð�ȭ
        Gizmos.color = isPulling ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // ���� �Ÿ� �ð�ȭ
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, releaseDistance);
    }
}