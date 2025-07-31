using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// VR 활쏘기 컨트롤러 - XR Interaction Toolkit 기반
/// 활 시위 조작, 화살 발사, 물리 시뮬레이션을 담당
/// ArrowLauncher와 ArrowImpactHandler와 완벽하게 연동됩니다.
/// </summary>
public class BowController : MonoBehaviour
{
    [Tooltip("활의 위치")]
    [SerializeField] private Transform _bowPosition;
    [Header("String & Nocking")]
    [Tooltip("활 시위를 표시할 라인 렌더러")]
    [SerializeField] private LineRenderer bowStringRenderer;

    [Tooltip("시위 시작점")]
    [SerializeField] private Transform stringStartPoint;

    [Tooltip("시위 끝점")]
    [SerializeField] private Transform stringEndPoint;

    [Tooltip("화살을 끼우는 소켓")]
    [SerializeField] private XRSocketInteractor nockSocket;

    [Header("Arrow System")]
    [Tooltip("화살 프리팹")]
    [SerializeField] private GameObject arrowPrefab;

    [Tooltip("화살 생성 위치")]
    [SerializeField] private Transform arrowSpawnPoint;

    [Tooltip("자동 화살 생성 간격 (초)")]
    [SerializeField] private float arrowSpawnInterval = 2f;

    [Tooltip("최대 보유 화살 수")]
    [SerializeField] private int maxArrows = 10;

    [Tooltip("시위 당김 시 화살 자동 생성 활성화")]
    [SerializeField] private bool autoSpawnOnPull = true;

    [Tooltip("시위 당김 시 화살 생성 딜레이 (초)")]
    [SerializeField] private float pullSpawnDelay = 0.5f;

    [Header("Shooting")]
    [Tooltip("발사 힘 배수")]
    [SerializeField] private float shootingForceMultiplier = 25f;

    [Tooltip("최대 당김 거리")]
    [SerializeField] private float maxPullDistance = 0.6f;

    [Tooltip("시위 장력 배수")]
    [SerializeField] private float stringTension = 1.2f;

    [Tooltip("당김 거리 제한 활성화")]
    [SerializeField] private bool enablePullLimit = true;

    [Tooltip("당김 거리 제한 시 시각적 피드백")]
    [SerializeField] private bool enableVisualFeedback = true;

    [Tooltip("당김 거리 제한 시 색상 변화")]
    [SerializeField] private Color maxPullColor = Color.red;

    [Tooltip("당김 거리 제한 시 진동 피드백")]
    [SerializeField] private bool enableHapticFeedback = true;

    [Tooltip("당김 거리 제한 시 진동 강도")]
    [SerializeField] private float hapticIntensity = 0.5f;

    [Header("Visual & Audio")]
    [Tooltip("시위 당김 사운드")]
    [SerializeField] private AudioClip pullSound;

    [Tooltip("화살 발사 사운드")]
    [SerializeField] private AudioClip releaseSound;

    [Tooltip("화살 장착 사운드")]
    [SerializeField] private AudioClip nockSound;

    [Tooltip("화살 생성 사운드")]
    [SerializeField] private AudioClip arrowSpawnSound;

    [Tooltip("시위 색상")]
    [SerializeField] private Color stringColor = Color.white;

    [Tooltip("시위 두께")]
    [SerializeField] private float stringWidth = 0.005f;

    [Tooltip("화살 생성 시 파티클 효과")]
    [SerializeField] private ParticleSystem arrowSpawnEffect;

    [Header("Debug")]
    [Tooltip("디버그 로그 활성화")]
    [SerializeField] private bool enableDebugLogs = true;

    [Tooltip("양손 동시 상호작용 허용")]
    [SerializeField] private bool allowDualHandInteraction = true;

    [Tooltip("활 잡기 감지 거리")]
    [SerializeField] private float bowGrabDistance = 0.3f;

    [Tooltip("시위 터치 감지 거리")]
    [SerializeField] private float stringTouchDistance = 0.1f;

    // 내부 변수
    private IXRSelectInteractable nockedArrow = null;
    private bool isArrowNocked = false;
    //private bool isStringPulled = false;
    //private Transform pullingHand = null;
    //private float currentPullDistance = 0f;
    private Vector3 originalStringPosition;
    private AudioSource audioSource;
    private int currentArrowCount = 0;
    private Coroutine arrowSpawnCoroutine;
    private XRPullInteractable stringPullInteractable;
    [Header("References")]
    public XRBaseInteractor leftController;
    public XRBaseInteractor rightController;
    public Transform stringTouchArea;
    public float stringTouchRadius = 0.1f;
    private bool isLeftHolding = false;
    private bool wasRightTouching = false;
    private bool isStringBeingPulled = false;
    private Coroutine pullSpawnCoroutine = null;

    // 이벤트
    public System.Action<float> OnPullStrengthChanged;
    public System.Action OnArrowReleased;
    public System.Action<int> OnArrowCountChanged;

    private void Awake()
    {
        InitializeBow();
    }

    private void OnDestroy()
    {
        // 이벤트 리스너 제거
        if (nockSocket != null)
        {
            nockSocket.selectEntered.RemoveListener(OnArrowNocked);
            nockSocket.selectExited.RemoveListener(OnArrowRemoved);
        }

        if (stringPullInteractable != null)
        {
            stringPullInteractable.PullActionReleased -= OnStringReleased;
            stringPullInteractable.selectEntered.RemoveListener(OnStringPullStarted);
            stringPullInteractable.selectExited.RemoveListener(OnStringPullCanceled);
        }
    }

    /// <summary>
    /// 활 초기화
    /// </summary>
    private void InitializeBow()
    {
        // AudioSource 컴포넌트 찾기 또는 추가
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // XR Interaction 설정
        SetupXRInteraction();

        // 시위 초기화
        SetupBowString();

        // 이벤트 리스너 등록
        if (nockSocket != null)
        {
            nockSocket.selectEntered.AddListener(OnArrowNocked);
            nockSocket.selectExited.AddListener(OnArrowRemoved);
        }

        // 시위 당김 감지 설정
        SetupStringPullDetection();

        // 자동 화살 생성 시작
        StartArrowSpawning();

        if (enableDebugLogs)
            Debug.Log("향상된 활 컨트롤러가 초기화되었습니다.");
    }

    /// <summary>
    /// XR Interaction 설정
    /// </summary>
    private void SetupXRInteraction()
    {
        // XRGrabInteractable 컴포넌트 찾기 또는 추가
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = gameObject.AddComponent<XRGrabInteractable>();
        }

        // 양손 동시 상호작용 허용 설정
        if (allowDualHandInteraction)
        {
            // Multiple Interactors Select Mode 설정
            grabInteractable.selectMode = InteractableSelectMode.Multiple;
            
            // 최대 선택 가능한 인터랙터 수 설정
            //grabInteractable.maxInteractors = 2;
            
            if (enableDebugLogs)
                Debug.Log("양손 동시 상호작용이 활성화되었습니다.");
        }
        else
        {
            // 단일 선택 모드
            grabInteractable.selectMode = InteractableSelectMode.Single;
            //grabInteractable.maxInteractors = 1;
        }

        // 상호작용 레이어 설정 (필요한 경우)
        // grabInteractable.interactionLayers = InteractionLayerMask.GetMask("Default");
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
        bowStringRenderer.useWorldSpace = false;

        // 시위 기본 위치 설정
        if (stringStartPoint != null && stringEndPoint != null)
        {
            ResetBowString();
        }
    }

    /// <summary>
    /// 시위 당김 감지 설정
    /// </summary>
    private void SetupStringPullDetection()
    {
        // XRPullInteractable 컴포넌트 찾기 또는 추가
        stringPullInteractable = GetComponent<XRPullInteractable>();
        if (stringPullInteractable == null)
        {
            stringPullInteractable = gameObject.AddComponent<XRPullInteractable>();
        }

        // 시위 당김 이벤트 등록
        stringPullInteractable.PullActionReleased += OnStringReleased;
        stringPullInteractable.selectEntered.AddListener(OnStringPullStarted);
        stringPullInteractable.selectExited.AddListener(OnStringPullCanceled);
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

        // 게임 시작 시 초기 화살 생성
        StartCoroutine(InitialArrowSpawn());
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
    /// 게임 시작 시 초기 화살 생성
    /// </summary>
    private IEnumerator InitialArrowSpawn()
    {
        yield return new WaitForSeconds(0.5f); // 게임 시작 후 잠시 대기
        
        // 초기 화살 3개 생성
        for (int i = 0; i < 3 && currentArrowCount < maxArrows; i++)
        {
            SpawnArrowInHand();
            yield return new WaitForSeconds(0.2f); // 화살 간 생성 간격
        }
    }

    /// <summary>
    /// 시위 당김 시 화살 생성 코루틴
    /// </summary>
    private IEnumerator SpawnArrowOnPull()
    {
        yield return new WaitForSeconds(pullSpawnDelay);
        
        if (isStringBeingPulled && currentArrowCount < maxArrows)
        {
            SpawnArrowInHand();
            
            if (enableDebugLogs)
                Debug.Log("시위 당김으로 인한 화살 생성!");
        }
    }

    /// <summary>
    /// 손에 화살 생성
    /// </summary>
    private void SpawnArrowInHand()
    {
        if (arrowPrefab != null)
        {
            // 라인렌더러의 인덱스 1번 위치를 화살 스폰 위치로 사용
            Vector3 spawnPosition = GetStringCenterPosition();
            Quaternion spawnRotation = GetStringCenterRotation();
            
            GameObject newArrow = Instantiate(arrowPrefab, spawnPosition, spawnRotation);
            SetupArrowComponents(newArrow);
            currentArrowCount++;
            OnArrowCountChanged?.Invoke(currentArrowCount);

            // 화살 생성 시 시각적/청각적 피드백
            PlayArrowSpawnFeedback();

            if (enableDebugLogs)
            {
                Debug.Log($"화살이 생성되었습니다. 현재 화살 수: {currentArrowCount}");
                Debug.Log($"화살 생성 위치: {spawnPosition}");
                Debug.Log($"화살 생성 회전: {spawnRotation.eulerAngles}");
            }
        }
    }

    /// <summary>
    /// 라인렌더러의 인덱스 1번 위치를 가져옵니다 (시위 중앙)
    /// </summary>
    /// <returns>시위 중앙 위치</returns>
    private Vector3 GetStringCenterPosition()
    {
        if (bowStringRenderer != null)
        {
            if (bowStringRenderer.positionCount >= 3)
            {
                // 3개 포인트일 때는 인덱스 1번 사용 (당김 상태)
                Vector3 stringCenter = bowStringRenderer.GetPosition(1);
                return stringCenter;
            }
            else if (bowStringRenderer.positionCount == 2)
            {
                // 2개 포인트일 때는 중간점 계산 (기본 상태)
                Vector3 pos0 = bowStringRenderer.GetPosition(0);
                Vector3 pos1 = bowStringRenderer.GetPosition(1);
                Vector3 stringCenter = (pos0 + pos1) * 0.5f;
                return stringCenter;
            }
        }
        
        // fallback: 기존 arrowSpawnPoint 사용
        if (arrowSpawnPoint != null)
        {
            return arrowSpawnPoint.position;
        }
        
        // fallback: 활의 현재 위치 사용
        return transform.position;
    }

    /// <summary>
    /// 라인렌더러의 인덱스 1번 위치에서의 회전을 계산합니다
    /// </summary>
    /// <returns>시위 중앙에서의 회전</returns>
    private Quaternion GetStringCenterRotation()
    {
        if (bowStringRenderer != null)
        {
            if (bowStringRenderer.positionCount >= 3)
            {
                // 3개 포인트일 때 (당김 상태)
                Vector3 pos0 = bowStringRenderer.GetPosition(0);
                Vector3 pos1 = bowStringRenderer.GetPosition(1);
                Vector3 pos2 = bowStringRenderer.GetPosition(2);
                
                // 시위의 평균 방향 계산
                Vector3 leftHalf = (pos1 - pos0).normalized;
                Vector3 rightHalf = (pos2 - pos1).normalized;
                Vector3 stringDirection = (leftHalf + rightHalf).normalized;
                
                // 시위가 당겨진 상태라면 활의 전방 방향을 우선 사용
                if (isStringBeingPulled || isArrowNocked)
                {
                    Vector3 bowForward = transform.forward;
                    Vector3 combinedDirection = (bowForward + stringDirection).normalized;
                    
                    if (combinedDirection != Vector3.zero)
                    {
                        return Quaternion.LookRotation(combinedDirection);
                    }
                }
                
                // 기본적으로 시위 방향 사용
                if (stringDirection != Vector3.zero)
                {
                    return Quaternion.LookRotation(stringDirection);
                }
            }
            else if (bowStringRenderer.positionCount == 2)
            {
                // 2개 포인트일 때 (기본 상태)
                Vector3 pos0 = bowStringRenderer.GetPosition(0);
                Vector3 pos1 = bowStringRenderer.GetPosition(1);
                Vector3 stringDirection = (pos1 - pos0).normalized;
                
                if (stringDirection != Vector3.zero)
                {
                    return Quaternion.LookRotation(stringDirection);
                }
            }
        }
        
        // fallback: 기존 arrowSpawnPoint의 회전 사용
        if (arrowSpawnPoint != null)
        {
            return arrowSpawnPoint.rotation;
        }
        
        // fallback: 활의 회전 사용
        return transform.rotation;
    }

    /// <summary>
    /// 화살 생성 시 피드백 재생
    /// </summary>
    private void PlayArrowSpawnFeedback()
    {
        // 화살 생성 사운드 재생
        if (arrowSpawnSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(arrowSpawnSound);
        }

        // 화살 생성 파티클 효과 재생 (라인렌더러 인덱스 1번 위치에서)
        if (arrowSpawnEffect != null)
        {
            Vector3 spawnPosition = GetStringCenterPosition();
            arrowSpawnEffect.transform.position = spawnPosition;
            arrowSpawnEffect.Play();
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
            launcher.Initialize(stringPullInteractable);

            // 이벤트 연결 (화살 카운트는 FireArrow나 Shoot에서 처리)
            launcher.OnArrowLaunched += () => {
                OnArrowReleased?.Invoke();

                if (enableDebugLogs)
                    Debug.Log("화살이 발사되었습니다.");
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

    /// <summary>
    /// 시위 당김 해제 이벤트 처리
    /// </summary>
    /// <param name="value">당김 강도 (0-1)</param>
    private void OnStringReleased(float value)
    {
        isStringBeingPulled = false;
        
        // 당김 중 화살 생성 코루틴 중지
        if (pullSpawnCoroutine != null)
        {
            StopCoroutine(pullSpawnCoroutine);
            pullSpawnCoroutine = null;
        }

        if (isArrowNocked && nockedArrow != null)
        {
            // 화살 발사
            FireArrow(value);

            // 시위 리셋
            ResetBowString();

            // 발사 사운드 재생
            if (releaseSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(releaseSound);
            }

            if (enableDebugLogs)
                Debug.Log($"화살 발사! 당김 강도: {value}");
        }
    }

    /// <summary>
    /// 시위 당김 시작 이벤트 처리
    /// </summary>
    /// <param name="args">당김 이벤트 인자</param>
    private void OnStringPullStarted(SelectEnterEventArgs args)
    {
        isStringBeingPulled = true;
        
        if (autoSpawnOnPull && currentArrowCount < maxArrows)
        {
            // 당김 시작 시 화살 생성
            if (pullSpawnCoroutine != null)
            {
                StopCoroutine(pullSpawnCoroutine);
            }
            pullSpawnCoroutine = StartCoroutine(SpawnArrowOnPull());
        }

        if (enableDebugLogs)
            Debug.Log("시위 당김 시작!");
    }

    /// <summary>
    /// 시위 당김 취소 이벤트 처리
    /// </summary>
    /// <param name="args">당김 이벤트 인자</param>
    private void OnStringPullCanceled(SelectExitEventArgs args)
    {
        isStringBeingPulled = false;
        
        // 당김 중 화살 생성 코루틴 중지
        if (pullSpawnCoroutine != null)
        {
            StopCoroutine(pullSpawnCoroutine);
            pullSpawnCoroutine = null;
        }

        if (enableDebugLogs)
            Debug.Log("시위 당김 취소!");
    }

    // Update 함수
    void Update()
    {
        gameObject.transform.position = _bowPosition.position;
        
        // 양손의 상호작용 상태 확인
        CheckHandInteractions();

        // 시위 당김 시각적 업데이트
        UpdateBowStringVisuals();

        // 화살 생성 로직
        HandleArrowSpawning();
    }

    /// <summary>
    /// 양손의 상호작용 상태 확인
    /// </summary>
    private void CheckHandInteractions()
    {
        // 왼손이 활을 잡고 있는지 확인 (더 유연한 방식)
        isLeftHolding = IsHandHoldingBow(leftController);
        
        // 오른손이 시위를 잡고 있는지 확인
        bool isRightTouching = IsHandTouchingString(rightController);
        
        // 이전 상태와 비교하여 변화 감지
        if (isLeftHolding && isRightTouching && !wasRightTouching)
        {
            if (enableDebugLogs)
                Debug.Log("양손이 모두 활과 시위를 잡고 있습니다!");
        }
        
        wasRightTouching = isRightTouching;
    }

    /// <summary>
    /// 손이 활을 잡고 있는지 확인
    /// </summary>
    /// <param name="controller">확인할 컨트롤러</param>
    /// <returns>활을 잡고 있으면 true</returns>
    private bool IsHandHoldingBow(XRBaseInteractor controller)
    {
        if (controller == null) return false;
        
        // 1. hasSelection으로 확인
        if (controller.hasSelection)
        {
            // 선택된 오브젝트가 활인지 확인
            IXRSelectInteractable selectedObject = controller.firstInteractableSelected;
            if (selectedObject != null && selectedObject.transform == transform)
            {
                return true;
            }
        }
        
        // 2. 활과의 거리로 확인 (fallback)
        float distanceToBow = Vector3.Distance(controller.transform.position, transform.position);
        return distanceToBow < bowGrabDistance; // 잡기 거리 사용
    }

    /// <summary>
    /// 손이 시위를 잡고 있는지 확인
    /// </summary>
    /// <param name="controller">확인할 컨트롤러</param>
    /// <returns>시위를 잡고 있으면 true</returns>
    private bool IsHandTouchingString(XRBaseInteractor controller)
    {
        if (controller == null) return false;
        
        // 1. stringTouchArea가 있으면 거리로 확인
        if (stringTouchArea != null)
        {
            float dist = Vector3.Distance(controller.transform.position, stringTouchArea.position);
            return dist <= stringTouchDistance; // 시위 터치 거리 사용
        }
        
        // 2. 라인렌더러의 중앙점으로 확인 (fallback)
        if (bowStringRenderer != null)
        {
            Vector3 stringCenter = GetStringCenterPosition();
            float dist = Vector3.Distance(controller.transform.position, stringCenter);
            return dist <= stringTouchDistance; // 시위 터치 거리 사용
        }
        
        return false;
    }

    /// <summary>
    /// 시위 시각적 업데이트 및 당김 거리 제한 처리
    /// </summary>
    private void UpdateBowStringVisuals()
    {
        // 왼손이 활을 잡고 있고 오른손이 시위를 잡고 있을 때
        if (isLeftHolding && IsHandTouchingString(rightController))
        {
            // 오른손이 시위를 당기고 있을 때 시위 위치 업데이트
            Vector3 pullPosition = rightController.transform.position;
            
            // 당김 거리 제한 적용
            if (enablePullLimit)
            {
                pullPosition = ApplyPullDistanceLimit(pullPosition);
            }
            
            UpdateBowString(pullPosition);
        }
        else if (isArrowNocked && nockedArrow != null)
        {
            // 화살이 장착되어 있을 때 화살을 잡고 있는 손의 위치로 시위 업데이트
            IXRSelectInteractor hand = nockedArrow.firstInteractorSelecting;
            if (hand != null)
            {
                Vector3 pullPosition = hand.transform.position;
                
                // 당김 거리 제한 적용
                if (enablePullLimit)
                {
                    pullPosition = ApplyPullDistanceLimit(pullPosition);
                }
                
                UpdateBowString(pullPosition);
            }
        }
        else
        {
            // 시위를 당기지 않을 때 기본 위치로 리셋
            ResetBowString();
        }
    }

    /// <summary>
    /// 당김 거리 제한을 적용합니다.
    /// </summary>
    /// <param name="pullPosition">원래 당김 위치</param>
    /// <returns>제한된 당김 위치</returns>
    private Vector3 ApplyPullDistanceLimit(Vector3 pullPosition)
    {
        // 시위 중앙 위치 계산
        Vector3 stringCenter = (stringStartPoint.position + stringEndPoint.position) * 0.5f;
        
        // 당김 방향과 거리 계산
        Vector3 pullDirection = (pullPosition - stringCenter).normalized;
        float currentDistance = Vector3.Distance(pullPosition, stringCenter);
        
        // 최대 당김 거리로 제한
        float clampedDistance = Mathf.Clamp(currentDistance, 0f, maxPullDistance);
        
        // 제한된 위치 반환
        return stringCenter + (pullDirection * clampedDistance);
    }

    /// <summary>
    /// 화살 생성 로직 처리
    /// </summary>
    private void HandleArrowSpawning()
    {
        // 조건 충족 시 화살 생성 (기존 로직)
        if (isLeftHolding && IsHandTouchingString(rightController) && !wasRightTouching && currentArrowCount < maxArrows)
        {
            SpawnArrow();
        }

        // 시위 당김 중일 때 추가 화살 생성 (새로운 로직)
        if (isStringBeingPulled && autoSpawnOnPull && currentArrowCount < maxArrows)
        {
            // 당김 중일 때는 일정 간격으로 화살 생성
            if (pullSpawnCoroutine == null)
            {
                pullSpawnCoroutine = StartCoroutine(SpawnArrowOnPull());
            }
        }
    }

    // 화살을 소켓에 끼우고 있을 때 호출되는 함수
    private void OnArrowNocked(SelectEnterEventArgs args)
    {
        // 디버그 로그 추가
        if (enableDebugLogs)
            Debug.Log("화살을 소켓에 끼우고 있는 것이 감지되었습니다! (OnArrowNocked 호출)");

        nockedArrow = args.interactableObject;
        isArrowNocked = true;

        // 화살을 소켓에서 제거할 때(발사되거나 그냥 제거될 때) Shoot 함수를 호출하도록 이벤트를 설정
        nockedArrow.selectExited.AddListener(Shoot);

        //if (enableDebugLogs)
        //    Debug.Log($"화살 장착 완료: {nockedArrow.name}");
    }

    // 화살을 소켓에서 제거할 때(발사되거나 그냥 제거될 때) 호출
    private void OnArrowRemoved(SelectExitEventArgs args)
    {
        // 이벤트 리스너 제거
        if (args.interactableObject == nockedArrow)
        {
            nockedArrow.selectExited.RemoveListener(Shoot);
            ResetBowString();
            nockedArrow = null;
            isArrowNocked = false;

            if (enableDebugLogs)
                Debug.Log("화살이 제거되었습니다.");
        }
    }

    // 화살 발사 함수
    private void Shoot(SelectExitEventArgs args)
    {
        if (enableDebugLogs)
            Debug.Log("화살 발사! (Shoot 함수 호출)");

        if (nockedArrow == null) return;

        // 당김 거리를 계산합니다 (손과 소켓 사이의 거리)
        float pullDistance = Vector3.Distance(args.interactorObject.transform.position, nockSocket.transform.position);
        
        // 당김 거리 제한 적용
        float clampedPullDistance = Mathf.Clamp(pullDistance, 0f, maxPullDistance);
        
        // 당김 강도 계산 (0-1)
        float pullStrength = clampedPullDistance / maxPullDistance;
        
        // 발사 힘 계산
        float finalForce = pullStrength * shootingForceMultiplier;

        if (enableDebugLogs)
            Debug.Log($"당김 거리: {pullDistance:F3}, 제한된 거리: {clampedPullDistance:F3}, 당김 강도: {pullStrength:F2}, 최종 힘: {finalForce:F2}");

        // 화살을 소켓에서 분리하고 물리 활성화
        Rigidbody arrowRigidbody = nockedArrow.transform.GetComponent<Rigidbody>();
        if (arrowRigidbody != null)
        {
            // 화살을 소켓에서 분리
            nockedArrow.transform.SetParent(null);
            
            // 물리 활성화
            arrowRigidbody.isKinematic = false;
            arrowRigidbody.useGravity = true;

            // 발사 방향 계산 (활의 전방 방향)
            Vector3 shootDirection = transform.forward;

            // 화살에 힘 적용
            arrowRigidbody.AddForce(shootDirection * finalForce, ForceMode.Impulse);
        }

        // 화살 카운트 감소
        currentArrowCount--;
        OnArrowCountChanged?.Invoke(currentArrowCount);
        OnArrowReleased?.Invoke();

        // 발사 사운드 재생
        if (releaseSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(releaseSound);
        }

        // 화살 상태 리셋
        nockedArrow = null;
        isArrowNocked = false;

        // 시위 리셋
        ResetBowString();
    }
    void SpawnArrow()
    {
        if (currentArrowCount < maxArrows)
        {
            SpawnArrowInHand();
        }
    }
    /// <summary>
    /// 화살을 발사합니다.
    /// </summary>
    /// <param name="pullStrength">당김 강도 (0-1)</param>
    private void FireArrow(float pullStrength)
    {
        if (nockedArrow == null) return;

        // 화살의 Rigidbody 가져오기
        Rigidbody arrowRigidbody = nockedArrow.transform.GetComponent<Rigidbody>();
        if (arrowRigidbody == null) return;

        // 당김 거리에 따른 발사 힘 계산
        float clampedPullStrength = Mathf.Clamp(pullStrength, 0f, 1f);
        float finalForce = clampedPullStrength * shootingForceMultiplier;

        // 화살을 소켓에서 분리
        nockedArrow.transform.SetParent(null);

        // 물리 활성화
        arrowRigidbody.isKinematic = false;
        arrowRigidbody.useGravity = true;

        // 발사 방향 계산 (활의 전방 방향)
        Vector3 shootDirection = transform.forward;

        // 화살에 힘 적용
        arrowRigidbody.AddForce(shootDirection * finalForce, ForceMode.Impulse);

        // 화살 카운트 감소
        currentArrowCount--;
        OnArrowCountChanged?.Invoke(currentArrowCount);
        OnArrowReleased?.Invoke();

        // 발사 사운드 재생
        if (releaseSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(releaseSound);
        }

        // 디버그 로그
        if (enableDebugLogs)
        {
            Debug.Log($"화살 발사! 당김 강도: {clampedPullStrength:F2}, 발사 힘: {finalForce:F2}");
        }

        // 화살 상태 리셋
        nockedArrow = null;
        isArrowNocked = false;
    }

    // 시위(Line Renderer)를 기본 위치로 되돌림
    private void ResetBowString()
    {
        if (bowStringRenderer != null && stringStartPoint != null && stringEndPoint != null)
        {
            bowStringRenderer.positionCount = 2;
            bowStringRenderer.SetPosition(0, stringStartPoint.position);
            bowStringRenderer.SetPosition(1, stringEndPoint.position);
        }
    }

    /// <summary>
    /// 시위를 당긴 위치로 업데이트합니다.
    /// </summary>
    /// <param name="pullPosition">당김 위치</param>
    private void UpdateBowString(Vector3 pullPosition)
    {
        if (bowStringRenderer == null) return;

        // 시위 중앙 위치 계산
        Vector3 stringCenter = (stringStartPoint.position + stringEndPoint.position) * 0.5f;
        
        // 당김 거리 계산
        float currentDistance = Vector3.Distance(pullPosition, stringCenter);
        float clampedDistance = Mathf.Clamp(currentDistance, 0f, maxPullDistance);
        
        // 당김 강도 계산 (0-1)
        float pullStrength = clampedDistance / maxPullDistance;
        
        // 이벤트 호출
        OnPullStrengthChanged?.Invoke(pullStrength);

        // 시위 위치 업데이트 (3점 라인 렌더러)
        bowStringRenderer.positionCount = 3;
        bowStringRenderer.SetPosition(0, stringStartPoint.position);
        bowStringRenderer.SetPosition(1, pullPosition);
        bowStringRenderer.SetPosition(2, stringEndPoint.position);

        // 당김 거리 제한 활성화 시 시각적 피드백
        if (enablePullLimit && enableVisualFeedback)
        {
            if (clampedDistance >= maxPullDistance * 0.95f) // 95% 이상 당겼을 때
            {
                // 최대 당김 시 색상 변화
                bowStringRenderer.startColor = maxPullColor;
                bowStringRenderer.endColor = maxPullColor;
                
                // 진동 피드백 (간단한 구현)
                if (enableHapticFeedback && enableDebugLogs)
                {
                    Debug.Log("최대 당김 거리 도달! 진동 피드백 발생");
                }
            }
            else
            {
                // 일반 당김 시 기본 색상
                bowStringRenderer.startColor = stringColor;
                bowStringRenderer.endColor = stringColor;
            }
        }
        else
        {
            // 시각적 피드백 비활성화 시 기본 색상
            bowStringRenderer.startColor = stringColor;
            bowStringRenderer.endColor = stringColor;
        }

        if (enableDebugLogs)
        {
            Debug.Log($"시위 업데이트 - 거리: {clampedDistance:F3}, 강도: {pullStrength:F2}");
        }
    }

    // 인스펙터에서 화살 생성 (테스트용)
    [ContextMenu("Create Arrow")]
    public void CreateArrow()
    {
        if (arrowPrefab != null)
        {
            // 라인렌더러의 인덱스 1번 위치를 화살 스폰 위치로 사용
            Vector3 spawnPosition = GetStringCenterPosition();
            Quaternion spawnRotation = GetStringCenterRotation();
            
            GameObject newArrow = Instantiate(arrowPrefab, spawnPosition, spawnRotation);
            SetupArrowComponents(newArrow);
            Debug.Log("화살이 수동으로 생성되었습니다.");
        }
        else
        {
            Debug.LogError("화살 프리팹이 설정되지 않았습니다!");
        }
    }

    // 상태 정보 반환
    public bool IsArrowNocked() => isArrowNocked;

    public float GetPullDistance()
    {
        if (isArrowNocked && nockedArrow != null)
        {
            IXRSelectInteractor hand = nockedArrow.firstInteractorSelecting;
            if (hand != null)
            {
                return Vector3.Distance(hand.transform.position, nockSocket.transform.position);
            }
        }
        return 0f;
    }

    public int GetCurrentArrowCount() => currentArrowCount;

    public float GetMaxPullDistance() => maxPullDistance;

    /// <summary>
    /// 화살 수를 UI에 표시 (외부에서 호출)
    /// </summary>
    public void UpdateArrowCountUI()
    {
        OnArrowCountChanged?.Invoke(currentArrowCount);
    }

    /// <summary>
    /// 수동으로 화살 생성 (UI 버튼 등에서 호출)
    /// </summary>
    public void ManualSpawnArrow()
    {
        if (currentArrowCount < maxArrows)
        {
            SpawnArrowInHand();
        }
    }

    /// <summary>
    /// 모든 화살 제거 (디버그용)
    /// </summary>
    [ContextMenu("Clear All Arrows")]
    public void ClearAllArrows()
    {
        currentArrowCount = 0;
        OnArrowCountChanged?.Invoke(currentArrowCount);
        
        if (enableDebugLogs)
            Debug.Log("모든 화살이 제거되었습니다.");
    }

    /// <summary>
    /// 라인렌더러 인덱스 1번 위치 디버그 (인스펙터에서 테스트용)
    /// </summary>
    [ContextMenu("Debug String Center Position")]
    public void DebugStringCenterPosition()
    {
        Vector3 centerPos = GetStringCenterPosition();
        Quaternion centerRot = GetStringCenterRotation();
        
        Debug.Log($"=== 라인렌더러 인덱스 1번 위치 디버그 ===");
        Debug.Log($"위치: {centerPos}");
        Debug.Log($"회전: {centerRot.eulerAngles}");
        Debug.Log($"시위 당김 상태: {isStringBeingPulled}");
        Debug.Log($"화살 장착 상태: {isArrowNocked}");
        
        if (bowStringRenderer != null)
        {
            Debug.Log($"라인렌더러 위치 수: {bowStringRenderer.positionCount}");
            for (int i = 0; i < bowStringRenderer.positionCount; i++)
            {
                Debug.Log($"인덱스 {i}: {bowStringRenderer.GetPosition(i)}");
            }
        }
        Debug.Log("=======================================");
    }

    /// <summary>
    /// 양손 상호작용 상태 디버그 (인스펙터에서 테스트용)
    /// </summary>
    [ContextMenu("Debug Hand Interaction")]
    public void DebugHandInteraction()
    {
        Debug.Log($"=== 양손 상호작용 상태 디버그 ===");
        
        // 왼손 상태
        bool leftHolding = IsHandHoldingBow(leftController);
        Debug.Log($"왼손 활 잡기: {leftHolding}");
        if (leftController != null)
        {
            Debug.Log($"왼손 hasSelection: {leftController.hasSelection}");
            //Debug.Log($"왼손 선택된 오브젝트: {(leftController.firstInteractableSelected != null ? leftController.firstInteractableSelected.name : "없음")}");
            Debug.Log($"왼손 위치: {leftController.transform.position}");
        }
        
        // 오른손 상태
        bool rightTouching = IsHandTouchingString(rightController);
        Debug.Log($"오른손 시위 터치: {rightTouching}");
        if (rightController != null)
        {
            Debug.Log($"오른손 hasSelection: {rightController.hasSelection}");
            Debug.Log($"오른손 위치: {rightController.transform.position}");
        }
        
        // 활 상태
        Debug.Log($"활 위치: {transform.position}");
        Debug.Log($"활 회전: {transform.rotation.eulerAngles}");
        
        // 시위 상태
        if (stringTouchArea != null)
        {
            Debug.Log($"시위 터치 영역 위치: {stringTouchArea.position}");
        }
        
        // 거리 정보
        if (leftController != null)
        {
            float leftDistance = Vector3.Distance(leftController.transform.position, transform.position);
            Debug.Log($"왼손-활 거리: {leftDistance:F3} (임계값: {bowGrabDistance})");
        }
        
        if (rightController != null && stringTouchArea != null)
        {
            float rightDistance = Vector3.Distance(rightController.transform.position, stringTouchArea.position);
            Debug.Log($"오른손-시위 거리: {rightDistance:F3} (임계값: {stringTouchDistance})");
        }
        
        Debug.Log("=======================================");
    }

    /// <summary>
    /// Gizmos로 시각적 디버그 정보 표시
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (!enableDebugLogs) return;

        // 활 잡기 감지 영역
        Gizmos.color = isLeftHolding ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, bowGrabDistance);
        
        // 시위 터치 감지 영역
        if (stringTouchArea != null)
        {
            Gizmos.color = IsHandTouchingString(rightController) ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(stringTouchArea.position, stringTouchDistance);
        }
        else if (bowStringRenderer != null)
        {
            Vector3 stringCenter = GetStringCenterPosition();
            Gizmos.color = IsHandTouchingString(rightController) ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(stringCenter, stringTouchDistance);
        }

        // 양손 위치 표시
        if (leftController != null)
        {
            Gizmos.color = isLeftHolding ? Color.green : Color.red;
            Gizmos.DrawSphere(leftController.transform.position, 0.02f);
        }
        
        if (rightController != null)
        {
            Gizmos.color = IsHandTouchingString(rightController) ? Color.green : Color.red;
            Gizmos.DrawSphere(rightController.transform.position, 0.02f);
        }

        // 시위 라인 표시
        if (bowStringRenderer != null)
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < bowStringRenderer.positionCount - 1; i++)
            {
                Vector3 start = bowStringRenderer.GetPosition(i);
                Vector3 end = bowStringRenderer.GetPosition(i + 1);
                Gizmos.DrawLine(start, end);
            }
            
            // 중앙점 표시
            Vector3 center = GetStringCenterPosition();
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(center, 0.01f);
        }
    }

    /// <summary>
    /// 최대 당김 거리를 설정합니다.
    /// </summary>
    /// <param name="distance">새로운 최대 당김 거리</param>
    public void SetMaxPullDistance(float distance)
    {
        maxPullDistance = Mathf.Max(0.1f, distance);
        if (enableDebugLogs)
        {
            Debug.Log($"최대 당김 거리가 {maxPullDistance}로 설정되었습니다.");
        }
    }

    /// <summary>
    /// 당김 거리 제한을 토글합니다.
    /// </summary>
    public void TogglePullLimit()
    {
        enablePullLimit = !enablePullLimit;
        if (enableDebugLogs)
        {
            Debug.Log($"당김 거리 제한이 {(enablePullLimit ? "활성화" : "비활성화")}되었습니다.");
        }
    }

    /// <summary>
    /// 시각적 피드백을 토글합니다.
    /// </summary>
    public void ToggleVisualFeedback()
    {
        enableVisualFeedback = !enableVisualFeedback;
        if (enableDebugLogs)
        {
            Debug.Log($"시각적 피드백이 {(enableVisualFeedback ? "활성화" : "비활성화")}되었습니다.");
        }
    }

    /// <summary>
    /// 현재 당김 거리를 반환합니다.
    /// </summary>
    /// <returns>현재 당김 거리 (0-1)</returns>
    public float GetCurrentPullStrength()
    {
        if (!isArrowNocked && !isStringBeingPulled) return 0f;
        
        Vector3 stringCenter = (stringStartPoint.position + stringEndPoint.position) * 0.5f;
        Vector3 currentPullPosition;
        
        if (isArrowNocked && nockedArrow != null)
        {
            IXRSelectInteractor hand = nockedArrow.firstInteractorSelecting;
            if (hand != null)
            {
                currentPullPosition = hand.transform.position;
            }
            else
            {
                return 0f;
            }
        }
        else if (isStringBeingPulled && rightController != null)
        {
            currentPullPosition = rightController.transform.position;
        }
        else
        {
            return 0f;
        }
        
        float distance = Vector3.Distance(currentPullPosition, stringCenter);
        return Mathf.Clamp(distance / maxPullDistance, 0f, 1f);
    }

    /// <summary>
    /// 디버그용 당김 거리 테스트
    /// </summary>
    [ContextMenu("Test Pull Distance")]
    public void TestPullDistance()
    {
        float currentStrength = GetCurrentPullStrength();
        Debug.Log($"현재 당김 강도: {currentStrength:F2} ({(currentStrength * 100):F0}%)");
        
        if (currentStrength >= 0.95f)
        {
            Debug.Log("최대 당김 거리에 도달했습니다!");
        }
    }

    /// <summary>
    /// 당김 거리 제한 설정을 리셋합니다.
    /// </summary>
    [ContextMenu("Reset Pull Settings")]
    public void ResetPullSettings()
    {
        maxPullDistance = 0.6f;
        enablePullLimit = true;
        enableVisualFeedback = true;
        enableHapticFeedback = true;
        maxPullColor = Color.red;
        hapticIntensity = 0.5f;
        
        if (enableDebugLogs)
        {
            Debug.Log("당김 거리 제한 설정이 기본값으로 리셋되었습니다.");
        }
    }
}