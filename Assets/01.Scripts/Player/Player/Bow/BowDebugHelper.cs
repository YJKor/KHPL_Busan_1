using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// 활쏘기 시스템 디버그 및 문제 해결을 위한 헬퍼 스크립트
/// 활 시위가 잡히지 않는 문제를 진단하고 해결합니다.
/// </summary>
public class BowDebugHelper : MonoBehaviour
{
    [Header("Debug References")]
    [Tooltip("디버그할 활 컨트롤러")]
    [SerializeField] private BowController bowController;

    [Tooltip("디버그할 향상된 활 컨트롤러")]
    [SerializeField] private EnhancedBowController enhancedBowController;

    [Header("Debug Settings")]
    [Tooltip("디버그 모드 활성화")]
    [SerializeField] private bool enableDebugMode = true;

    [Tooltip("시위 감지 영역 시각화")]
    [SerializeField] private bool showStringDetectionArea = true;

    [Tooltip("상태 정보 표시")]
    [SerializeField] private bool showStatusInfo = true;

    [Header("Manual Testing")]
    [Tooltip("수동 시위 당김 테스트")]
    [SerializeField] private bool enableManualTesting = false;

    [Tooltip("수동 당김 강도 (0-1)")]
    [SerializeField] private float manualPullStrength = 0.5f;

    // 내부 변수
    private XRPullInteractable stringPullInteractable;
    private XRSocketInteractor nockSocket;
    private LineRenderer bowStringRenderer;
    private bool isInitialized = false;

    void Start()
    {
        InitializeDebugHelper();
    }

    void Update()
    {
        if (enableDebugMode)
        {
            UpdateDebugInfo();
        }

        if (enableManualTesting)
        {
            HandleManualTesting();
        }
    }

    /// <summary>
    /// 디버그 헬퍼 초기화
    /// </summary>
    private void InitializeDebugHelper()
    {
        // 활 컨트롤러 찾기
        if (bowController == null)
        {
            bowController = FindObjectOfType<BowController>();
        }

        if (enhancedBowController == null)
        {
            enhancedBowController = FindObjectOfType<EnhancedBowController>();
        }

        // 컴포넌트 찾기
        if (bowController != null)
        {
            stringPullInteractable = bowController.GetComponent<XRPullInteractable>();
            nockSocket = bowController.GetComponentInChildren<XRSocketInteractor>();
            bowStringRenderer = bowController.GetComponent<LineRenderer>();
        }

        isInitialized = true;
        Debug.Log("BowDebugHelper 초기화 완료");
    }

    /// <summary>
    /// 디버그 정보 업데이트
    /// </summary>
    private void UpdateDebugInfo()
    {
        if (!isInitialized) return;

        // 상태 정보 출력
        if (showStatusInfo)
        {
            LogSystemStatus();
        }

        // 시위 감지 영역 시각화
        if (showStringDetectionArea)
        {
            VisualizeStringDetectionArea();
        }
    }

    /// <summary>
    /// 시스템 상태 로그 출력
    /// </summary>
    private void LogSystemStatus()
    {
        Debug.Log("=== 활쏘기 시스템 상태 ===");
        
        // 활 컨트롤러 상태
        if (bowController != null)
        {
            Debug.Log($"BowController: 활성화됨");
            Debug.Log($"화살 장착: {bowController.IsArrowNocked()}");
            Debug.Log($"현재 화살 수: {bowController.GetCurrentArrowCount()}");
            Debug.Log($"당김 거리: {bowController.GetPullDistance():F3}");
        }
        else
        {
            Debug.LogWarning("BowController: 찾을 수 없음");
        }

        // 향상된 활 컨트롤러 상태
        if (enhancedBowController != null)
        {
            Debug.Log($"EnhancedBowController: 활성화됨");
            Debug.Log($"화살 장착: {enhancedBowController.IsArrowNocked()}");
            Debug.Log($"현재 화살 수: {enhancedBowController.GetCurrentArrowCount()}");
        }

        // XRPullInteractable 상태
        if (stringPullInteractable != null)
        {
            Debug.Log($"XRPullInteractable: 활성화됨");
            Debug.Log($"선택됨: {stringPullInteractable.isSelected}");
            Debug.Log($"상호작용 가능: {stringPullInteractable.interactionManager != null}");
        }
        else
        {
            Debug.LogError("XRPullInteractable: 찾을 수 없음 - 시위 당김이 작동하지 않을 수 있습니다!");
        }

        // 소켓 상태
        if (nockSocket != null)
        {
            Debug.Log($"NockSocket: 활성화됨");
            //Debug.Log($"선택됨: {nockSocket.isSelected}");
            Debug.Log($"상호작용 가능: {nockSocket.interactionManager != null}");
        }
        else
        {
            Debug.LogError("NockSocket: 찾을 수 없음 - 화살 장착이 작동하지 않을 수 있습니다!");
        }

        // LineRenderer 상태
        if (bowStringRenderer != null)
        {
            Debug.Log($"LineRenderer: 활성화됨");
            Debug.Log($"위치 수: {bowStringRenderer.positionCount}");
            Debug.Log($"활성화됨: {bowStringRenderer.enabled}");
        }
        else
        {
            Debug.LogWarning("LineRenderer: 찾을 수 없음 - 시위가 보이지 않을 수 있습니다!");
        }

        Debug.Log("========================");
    }

    /// <summary>
    /// 시위 감지 영역 시각화
    /// </summary>
    private void VisualizeStringDetectionArea()
    {
        if (stringPullInteractable == null) return;

        // 시위 감지 영역을 시각적으로 표시
        Vector3 center = stringPullInteractable.transform.position;
        float radius = 0.1f; // 감지 반경

        // Gizmos로 원형 영역 표시
        Debug.DrawLine(center + Vector3.up * radius, center - Vector3.up * radius, Color.red);
        Debug.DrawLine(center + Vector3.right * radius, center - Vector3.right * radius, Color.red);
        Debug.DrawLine(center + Vector3.forward * radius, center - Vector3.forward * radius, Color.red);
    }

    /// <summary>
    /// 수동 테스트 처리
    /// </summary>
    private void HandleManualTesting()
    {
        // 키보드 입력으로 수동 테스트
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestStringPull();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            TestArrowRelease();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CreateTestArrow();
        }
    }

    /// <summary>
    /// 시위 당김 테스트
    /// </summary>
    [ContextMenu("Test String Pull")]
    public void TestStringPull()
    {
        if (stringPullInteractable != null)
        {
            Debug.Log($"시위 당김 테스트 - 강도: {manualPullStrength}");
            
            // 수동으로 당김 이벤트 발생
            var method = stringPullInteractable.GetType().GetMethod("PullActionReleased", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (method != null)
            {
                method.Invoke(stringPullInteractable, new object[] { manualPullStrength });
            }
        }
        else
        {
            Debug.LogError("XRPullInteractable이 없어서 테스트할 수 없습니다!");
        }
    }

    /// <summary>
    /// 화살 발사 테스트
    /// </summary>
    [ContextMenu("Test Arrow Release")]
    public void TestArrowRelease()
    {
        if (bowController != null)
        {
            Debug.Log("화살 발사 테스트");
            bowController.CreateArrow();
        }
        else
        {
            Debug.LogError("BowController가 없어서 테스트할 수 없습니다!");
        }
    }

    /// <summary>
    /// 테스트 화살 생성
    /// </summary>
    [ContextMenu("Create Test Arrow")]
    public void CreateTestArrow()
    {
        if (bowController != null)
        {
            Debug.Log("테스트 화살 생성");
            bowController.CreateArrow();
        }
    }

    /// <summary>
    /// 시위 감지 문제 진단
    /// </summary>
    [ContextMenu("Diagnose String Pull Issues")]
    public void DiagnoseStringPullIssues()
    {
        Debug.Log("=== 시위 당김 문제 진단 ===");

        // 1. XRPullInteractable 확인
        if (stringPullInteractable == null)
        {
            Debug.LogError("문제 1: XRPullInteractable 컴포넌트가 없습니다!");
            Debug.Log("해결책: 활 오브젝트에 XRPullInteractable 컴포넌트를 추가하세요.");
            return;
        }

        // 2. Interaction Manager 확인
        if (stringPullInteractable.interactionManager == null)
        {
            Debug.LogError("문제 2: Interaction Manager가 설정되지 않았습니다!");
            Debug.Log("해결책: XR Interaction Manager를 씬에 추가하고 활에 할당하세요.");
            return;
        }

        // 3. Collider 확인
        Collider stringCollider = stringPullInteractable.GetComponent<Collider>();
        if (stringCollider == null)
        {
            Debug.LogError("문제 3: 시위에 Collider가 없습니다!");
            Debug.Log("해결책: 활 시위 오브젝트에 Collider를 추가하세요.");
            return;
        }

        // 4. 시위 위치 확인
        if (stringPullInteractable.transform.position == Vector3.zero)
        {
            Debug.LogWarning("문제 4: 시위 위치가 (0,0,0)입니다!");
            Debug.Log("해결책: 시위를 적절한 위치로 이동하세요.");
        }

        // 5. XR Interactor 확인
        XRDirectInteractor[] interactors = FindObjectsOfType<XRDirectInteractor>();
        if (interactors.Length == 0)
        {
            Debug.LogError("문제 5: XR Interactor가 없습니다!");
            Debug.Log("해결책: XR Origin에 XR Direct Interactor를 추가하세요.");
            return;
        }

        Debug.Log("진단 완료: 모든 기본 설정이 올바릅니다.");
        Debug.Log("추가 확인사항:");
        Debug.Log("- VR 컨트롤러가 활 시위 근처에 있는지 확인");
        Debug.Log("- 컨트롤러의 버튼을 눌러 시위를 잡으려고 시도");
        Debug.Log("- 시위 감지 영역이 적절한 크기인지 확인");
    }

    /// <summary>
    /// 시위 감지 영역 크기 조정
    /// </summary>
    [ContextMenu("Adjust String Detection Area")]
    public void AdjustStringDetectionArea()
    {
        if (stringPullInteractable == null) return;

        // 시위 감지 영역을 더 크게 만들기
        Collider stringCollider = stringPullInteractable.GetComponent<Collider>();
        if (stringCollider != null)
        {
            if (stringCollider is SphereCollider sphereCollider)
            {
                sphereCollider.radius = 0.2f; // 반지름을 0.2로 증가
                Debug.Log("시위 감지 영역을 0.2로 확장했습니다.");
            }
            else if (stringCollider is BoxCollider boxCollider)
            {
                boxCollider.size = new Vector3(0.4f, 0.4f, 0.4f); // 크기를 0.4로 증가
                Debug.Log("시위 감지 영역을 0.4로 확장했습니다.");
            }
        }
    }

    /// <summary>
    /// 모든 컴포넌트 재설정
    /// </summary>
    [ContextMenu("Reset All Components")]
    public void ResetAllComponents()
    {
        Debug.Log("모든 컴포넌트를 재설정합니다...");

        // 활 컨트롤러 재초기화
        if (bowController != null)
        {
            // Awake 메서드 재호출
            var awakeMethod = bowController.GetType().GetMethod("Awake", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (awakeMethod != null)
            {
                awakeMethod.Invoke(bowController, null);
            }
        }

        // 향상된 활 컨트롤러 재초기화
        if (enhancedBowController != null)
        {
            var startMethod = enhancedBowController.GetType().GetMethod("Start", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (startMethod != null)
            {
                startMethod.Invoke(enhancedBowController, null);
            }
        }

        Debug.Log("컴포넌트 재설정 완료");
    }

    void OnDrawGizmosSelected()
    {
        if (!showStringDetectionArea || stringPullInteractable == null) return;

        // 시위 감지 영역을 Gizmos로 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(stringPullInteractable.transform.position, 0.1f);
        
        // 시위 방향 표시
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(stringPullInteractable.transform.position, stringPullInteractable.transform.forward * 0.2f);
    }
} 