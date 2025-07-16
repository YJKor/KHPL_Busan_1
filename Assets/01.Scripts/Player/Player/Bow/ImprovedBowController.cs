using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

/// <summary>
/// 개선된 활 컨트롤러 - 시위 당김 문제 해결
/// 더 안정적이고 직관적인 활쏘기 경험을 제공합니다.
/// </summary>
public class ImprovedBowController : MonoBehaviour
{
    [Header("Bow References")]
    [Tooltip("활 시위를 표시할 라인 렌더러")]
    [SerializeField] private LineRenderer bowStringRenderer;
    
    [Tooltip("시위 시작점")]
    [SerializeField] private Transform stringStartPoint;
    
    [Tooltip("시위 끝점")]
    [SerializeField] private Transform stringEndPoint;
    
    [Tooltip("화살을 끼우는 소켓")]
    [SerializeField] private XRSocketInteractor nockSocket;

    [Header("String Pull Detection")]
    [Tooltip("시위 당김 감지 영역")]
    [SerializeField] private Transform stringPullArea;
    
    [Tooltip("시위 당김 감지 반경")]
    [SerializeField] private float pullDetectionRadius = 0.15f;
    
    [Tooltip("시위 당김 해제 거리")]
    [SerializeField] private float pullReleaseDistance = 0.25f;

    [Header("Arrow System")]
    [Tooltip("화살 프리팹")]
    [SerializeField] private GameObject arrowPrefab;
    
    [Tooltip("화살 생성 위치")]
    [SerializeField] private Transform arrowSpawnPoint;
    
    [Tooltip("자동 화살 생성 간격 (초)")]
    [SerializeField] private float arrowSpawnInterval = 2f;
    
    [Tooltip("최대 보유 화살 수")]
    [SerializeField] private int maxArrows = 10;

    [Header("Shooting")]
    [Tooltip("발사 힘 배수")]
    [SerializeField] private float shootingForceMultiplier = 25f;
    
    [Tooltip("최대 당김 거리")]
    [SerializeField] private float maxPullDistance = 0.6f;
    
    [Tooltip("시위 장력 배수")]
    [SerializeField] private float stringTension = 1.2f;

    [Header("Visual & Audio")]
    [Tooltip("시위 당김 사운드")]
    [SerializeField] private AudioClip pullSound;
    
    [Tooltip("화살 발사 사운드")]
    [SerializeField] private AudioClip releaseSound;
    
    [Tooltip("화살 장착 사운드")]
    [SerializeField] private AudioClip nockSound;
    
    [Tooltip("시위 색상")]
    [SerializeField] private Color stringColor = Color.white;
    
    [Tooltip("시위 두께")]
    [SerializeField] private float stringWidth = 0.005f;

    [Header("Debug")]
    [Tooltip("디버그 로그 활성화")]
    [SerializeField] private bool enableDebugLogs = true;
    
    [Tooltip("시위 감지 영역 시각화")]
    [SerializeField] private bool showDetectionArea = true;

    // 내부 변수
    private IXRSelectInteractable nockedArrow = null;
    private bool isArrowNocked = false;
    private bool isStringPulled = false;
    private Transform pullingHand = null;
    private float currentPullDistance = 0f;
    private Vector3 originalStringPosition;
    private AudioSource audioSource;
    private int currentArrowCount = 0;
    private Coroutine arrowSpawnCoroutine;
    private SphereCollider pullDetectionCollider;
    private XRDirectInteractor[] handInteractors;

    // 이벤트
    public System.Action<float> OnPullStrengthChanged;
    public System.Action OnArrowReleased;
    public System.Action<int> OnArrowCountChanged;

    private void Awake()
    {
        InitializeImprovedBow();
    }

    private void OnDestroy()
    {
        // 이벤트 리스너 제거
        if (nockSocket != null)
        {
            nockSocket.selectEntered.RemoveListener(OnArrowNocked);
            nockSocket.selectExited.RemoveListener(OnArrowRemoved);
        }
    }

    /// <summary>
    /// 개선된 활 초기화
    /// </summary>
    private void InitializeImprovedBow()
    {
        // AudioSource 컴포넌트 찾기 또는 추가
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 시위 초기화
        SetupBowString();

        // 시위 당김 감지 영역 설정
        SetupStringPullDetection();

        // 이벤트 리스너 등록
        if (nockSocket != null)
        {
            nockSocket.selectEntered.AddListener(OnArrowNocked);
            nockSocket.selectExited.AddListener(OnArrowRemoved);
        }

        // 손 인터랙터 찾기
        FindHandInteractors();

        // 자동 화살 생성 시작
        StartArrowSpawning();

        if (enableDebugLogs)
            Debug.Log("개선된 활 컨트롤러가 초기화되었습니다.");
    }

    /// <summary>
    /// 활 시위 설정
    /// </summary>
    private void SetupBowString()
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

        // 시위 기본 위치 설정
        if (stringStartPoint != null && stringEndPoint != null)
        {
            originalStringPosition = (stringStartPoint.position + stringEndPoint.position) * 0.5f;
            ResetBowString();
        }
    }

    /// <summary>
    /// 시위 당김 감지 영역 설정
    /// </summary>
    private void SetupStringPullDetection()
    {
        // 시위 당김 감지 영역 생성
        if (stringPullArea == null)
        {
            GameObject pullArea = new GameObject("StringPullArea");
            pullArea.transform.SetParent(transform);
            pullArea.transform.localPosition = Vector3.zero;
            stringPullArea = pullArea.transform;
        }

        // SphereCollider 추가
        pullDetectionCollider = stringPullArea.gameObject.GetComponent<SphereCollider>();
        if (pullDetectionCollider == null)
        {
            pullDetectionCollider = stringPullArea.gameObject.AddComponent<SphereCollider>();
        }

        pullDetectionCollider.isTrigger = true;
        pullDetectionCollider.radius = pullDetectionRadius;

        // 시각적 표시를 위한 디버그 오브젝트
        if (showDetectionArea)
        {
            CreateDetectionAreaVisual();
        }
    }

    /// <summary>
    /// 감지 영역 시각화 생성
    /// </summary>
    private void CreateDetectionAreaVisual()
    {
        GameObject visualSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visualSphere.name = "StringDetectionVisual";
        visualSphere.transform.SetParent(stringPullArea);
        visualSphere.transform.localPosition = Vector3.zero;
        visualSphere.transform.localScale = Vector3.one * pullDetectionRadius * 2f;

        // 렌더러 설정
        Renderer renderer = visualSphere.GetComponent<Renderer>();
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

        // Collider 제거 (트리거만 사용)
        DestroyImmediate(visualSphere.GetComponent<Collider>());
    }

    /// <summary>
    /// 손 인터랙터 찾기
    /// </summary>
    private void FindHandInteractors()
    {
        handInteractors = FindObjectsOfType<XRDirectInteractor>();
        if (enableDebugLogs)
        {
            Debug.Log($"발견된 손 인터랙터 수: {handInteractors.Length}");
            foreach (var interactor in handInteractors)
            {
                Debug.Log($"인터랙터: {interactor.name}");
            }
        }
    }

    /// <summary>
    /// 자동 화살 생성 시작
    /// </summary>
    private void StartArrowSpawning()
    {
        if (arrowSpawnCoroutine != null)
        {
            StopCoroutine(arrowSpawnCoroutine);
        }
        arrowSpawnCoroutine = StartCoroutine(AutoArrowSpawn());
    }

    /// <summary>
    /// 자동 화살 생성 코루틴
    /// </summary>
    private IEnumerator AutoArrowSpawn()
    {
        while (currentArrowCount < maxArrows)
        {
            yield return new WaitForSeconds(arrowSpawnInterval);
            if (currentArrowCount < maxArrows)
            {
                SpawnArrowInHand();
            }
        }
    }

    /// <summary>
    /// 손에 화살 생성
    /// </summary>
    private void SpawnArrowInHand()
    {
        if (arrowPrefab != null && arrowSpawnPoint != null)
        {
            GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
            SetupArrowComponents(newArrow);
            currentArrowCount++;
            OnArrowCountChanged?.Invoke(currentArrowCount);

            if (enableDebugLogs)
                Debug.Log($"화살이 생성되었습니다. 현재 화살 수: {currentArrowCount}");
        }
    }

    /// <summary>
    /// 화살 컴포넌트 설정
    /// </summary>
    /// <param name="arrow">설정할 화살 오브젝트</param>
    private void SetupArrowComponents(GameObject arrow)
    {
        // ArrowLauncher 컴포넌트 설정
        ArrowLauncher launcher = arrow.GetComponent<ArrowLauncher>();
        if (launcher != null)
        {
            // 이벤트 연결
            launcher.OnArrowLaunched += () => {
                currentArrowCount--;
                OnArrowCountChanged?.Invoke(currentArrowCount);
                OnArrowReleased?.Invoke();
                
                if (enableDebugLogs)
                    Debug.Log($"화살이 발사되었습니다. 남은 화살 수: {currentArrowCount}");
            };
        }

        // ArrowImpactHandler 컴포넌트 확인
        ArrowImpactHandler impactHandler = arrow.GetComponent<ArrowImpactHandler>();
        if (impactHandler == null)
        {
            impactHandler = arrow.AddComponent<ArrowImpactHandler>();
        }

        // Rigidbody 설정
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Collider 설정
        Collider col = arrow.GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = false;
        }
    }

    // Update 함수
    void Update()
    {
        UpdateStringPullDetection();
        UpdateBowString();
    }

    /// <summary>
    /// 시위 당김 감지 업데이트
    /// </summary>
    private void UpdateStringPullDetection()
    {
        if (!isArrowNocked || nockedArrow == null) return;

        // 가장 가까운 손 찾기
        Transform closestHand = FindClosestHand();
        
        if (closestHand != null)
        {
            float distanceToHand = Vector3.Distance(stringPullArea.position, closestHand.position);
            
            // 시위 당김 감지 영역 내에 있는지 확인
            if (distanceToHand <= pullDetectionRadius)
            {
                if (!isStringPulled)
                {
                    StartStringPull(closestHand);
                }
                else
                {
                    UpdateStringPull(closestHand);
                }
            }
            else if (isStringPulled && distanceToHand > pullReleaseDistance)
            {
                ReleaseStringPull();
            }
        }
    }

    /// <summary>
    /// 가장 가까운 손 찾기
    /// </summary>
    /// <returns>가장 가까운 손의 Transform</returns>
    private Transform FindClosestHand()
    {
        if (handInteractors == null || handInteractors.Length == 0) return null;

        Transform closestHand = null;
        float closestDistance = float.MaxValue;

        foreach (var interactor in handInteractors)
        {
            if (interactor.isSelectActive)
            {
                float distance = Vector3.Distance(stringPullArea.position, interactor.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestHand = interactor.transform;
                }
            }
        }

        return closestHand;
    }

    /// <summary>
    /// 시위 당김 시작
    /// </summary>
    /// <param name="hand">당기는 손</param>
    private void StartStringPull(Transform hand)
    {
        isStringPulled = true;
        pullingHand = hand;
        currentPullDistance = 0f;

        if (enableDebugLogs)
            Debug.Log("시위 당김 시작");

        // 당김 사운드 재생
        if (pullSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pullSound);
        }
    }

    /// <summary>
    /// 시위 당김 업데이트
    /// </summary>
    /// <param name="hand">당기는 손</param>
    private void UpdateStringPull(Transform hand)
    {
        if (!isStringPulled || hand == null) return;

        currentPullDistance = Vector3.Distance(stringPullArea.position, hand.position);
        currentPullDistance = Mathf.Clamp(currentPullDistance, 0f, maxPullDistance);

        // 당김 강도 이벤트 호출
        float pullStrength = currentPullDistance / maxPullDistance;
        OnPullStrengthChanged?.Invoke(pullStrength);

        if (enableDebugLogs)
            Debug.Log($"시위 당김 업데이트 - 거리: {currentPullDistance:F3}, 강도: {pullStrength:F3}");
    }

    /// <summary>
    /// 시위 당김 해제
    /// </summary>
    private void ReleaseStringPull()
    {
        if (!isStringPulled) return;

        if (isArrowNocked && nockedArrow != null)
        {
            // 화살 발사
            FireArrow();
        }

        // 시위 리셋
        isStringPulled = false;
        pullingHand = null;
        currentPullDistance = 0f;
        ResetBowString();

        // 발사 사운드 재생
        if (releaseSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(releaseSound);
        }

        if (enableDebugLogs)
            Debug.Log("시위 당김 해제 - 화살 발사");
    }

    /// <summary>
    /// 화살 발사
    /// </summary>
    private void FireArrow()
    {
        if (nockedArrow == null) return;

        // ArrowLauncher 컴포넌트 찾기
        ArrowLauncher launcher = nockedArrow.transform.GetComponent<ArrowLauncher>();
        if (launcher != null)
        {
            // 수동으로 발사 처리
            Rigidbody arrowRigidbody = nockedArrow.transform.GetComponent<Rigidbody>();
            if (arrowRigidbody != null)
            {
                arrowRigidbody.isKinematic = false;
                arrowRigidbody.useGravity = true;

                Vector3 shootDirection = nockedArrow.transform.forward;
                float pullStrength = currentPullDistance / maxPullDistance;
                float finalForce = pullStrength * shootingForceMultiplier;
                
                arrowRigidbody.AddForce(shootDirection * finalForce, ForceMode.Impulse);

                if (enableDebugLogs)
                    Debug.Log($"화살 발사 - 힘: {finalForce:F2}, 방향: {shootDirection}");
            }
        }

        // 화살 상태 리셋
        nockedArrow = null;
        isArrowNocked = false;
    }

    // 화살을 소켓에 끼우고 있을 때 호출되는 함수
    private void OnArrowNocked(SelectEnterEventArgs args)
    {
        if (enableDebugLogs)
            Debug.Log("화살을 소켓에 끼우고 있는 것이 감지되었습니다!");

        nockedArrow = args.interactableObject;
        isArrowNocked = true;

        // 장착 사운드 재생
        if (nockSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(nockSound);
        }
    }

    // 화살을 소켓에서 제거할 때 호출
    private void OnArrowRemoved(SelectExitEventArgs args)
    {
        if (args.interactableObject == nockedArrow)
        {
            ResetBowString();
            nockedArrow = null;
            isArrowNocked = false;

            if (enableDebugLogs)
                Debug.Log("화살이 제거되었습니다.");
        }
    }

    // 시위(Line Renderer)를 기본 위치로 되돌림
    private void ResetBowString()
    {
        if (bowStringRenderer != null)
        {
            bowStringRenderer.positionCount = 2;
            bowStringRenderer.SetPosition(0, stringStartPoint.position);
            bowStringRenderer.SetPosition(1, stringEndPoint.position);
        }
    }

    // 시위를 당긴 손 위치로 업데이트
    private void UpdateBowString()
    {
        if (bowStringRenderer != null && isStringPulled && pullingHand != null)
        {
            // 당김 거리를 계산
            Vector3 direction = (pullingHand.position - stringPullArea.position).normalized;
            float distance = Vector3.Distance(stringPullArea.position, pullingHand.position);
            float clampedDistance = Mathf.Clamp(distance, 0f, maxPullDistance);
            Vector3 clampedPullPosition = stringPullArea.position + direction * clampedDistance;

            bowStringRenderer.positionCount = 3;
            bowStringRenderer.SetPosition(0, stringStartPoint.position);
            bowStringRenderer.SetPosition(1, clampedPullPosition);
            bowStringRenderer.SetPosition(2, stringEndPoint.position);
        }
        else if (bowStringRenderer != null && !isStringPulled)
        {
            ResetBowString();
        }
    }

    // 인스펙터에서 화살 생성 (테스트용)
    [ContextMenu("Create Arrow")]
    public void CreateArrow()
    {
        if (arrowPrefab != null && arrowSpawnPoint != null)
        {
            GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
            SetupArrowComponents(newArrow);
            Debug.Log("화살이 수동으로 생성되었습니다.");
        }
        else
        {
            Debug.LogError("화살 프리팹이나 생성 위치가 설정되지 않았습니다!");
        }
    }

    // 상태 정보 반환
    public bool IsArrowNocked() => isArrowNocked;
    public bool IsStringPulled() => isStringPulled;
    public float GetPullDistance() => currentPullDistance;
    public int GetCurrentArrowCount() => currentArrowCount;
    public float GetMaxPullDistance() => maxPullDistance;

    void OnDrawGizmosSelected()
    {
        if (!showDetectionArea || stringPullArea == null) return;

        // 시위 감지 영역을 Gizmos로 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(stringPullArea.position, pullDetectionRadius);
        
        // 시위 방향 표시
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(stringPullArea.position, transform.forward * 0.2f);
    }
} 