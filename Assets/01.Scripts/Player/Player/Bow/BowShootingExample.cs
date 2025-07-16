using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 완전한 활쏘기 시스템 사용 예시
/// ArrowLauncher, ArrowImpactHandler, BowController를 통합하여 사용하는 방법을 보여줍니다.
/// </summary>
public class BowShootingExample : MonoBehaviour
{
    [Header("Bow References")]
    [Tooltip("활 컨트롤러")]
    [SerializeField] private BowController bowController;

    [Header("UI Elements")]
    [Tooltip("당김 강도 표시 슬라이더")]
    [SerializeField] private Slider pullStrengthSlider;

    [Tooltip("화살 수 표시 텍스트")]
    [SerializeField] private Text arrowCountText;

    [Tooltip("점수 표시 텍스트")]
    [SerializeField] private Text scoreText;

    [Tooltip("발사 힘 표시 텍스트")]
    [SerializeField] private Text forceText;

    [Header("Target System")]
    [Tooltip("타겟 프리팹")]
    [SerializeField] private GameObject targetPrefab;

    [Tooltip("타겟 생성 위치들")]
    [SerializeField] private Transform[] targetSpawnPoints;

    [Tooltip("타겟 생성 간격 (초)")]
    [SerializeField] private float targetSpawnInterval = 5f;

    [Header("Game Settings")]
    [Tooltip("게임 시작 시 화살 수")]
    [SerializeField] private int startingArrowCount = 10;

    [Tooltip("자동 화살 보충")]
    [SerializeField] private bool autoRefillArrows = true;

    // 내부 변수
    private ScoreManager scoreManager;
    private Coroutine targetSpawnCoroutine;
    private int currentScore = 0;

    void Start()
    {
        InitializeBowShootingSystem();
    }

    /// <summary>
    /// 활쏘기 시스템 초기화
    /// </summary>
    private void InitializeBowShootingSystem()
    {
        // ScoreManager 찾기 또는 생성
        scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager == null)
        {
            GameObject scoreManagerObj = new GameObject("ScoreManager");
            scoreManager = scoreManagerObj.AddComponent<ScoreManager>();
        }

        // BowController 이벤트 연결
        if (bowController != null)
        {
            bowController.OnPullStrengthChanged += OnPullStrengthChanged;
            bowController.OnArrowReleased += OnArrowReleased;
            bowController.OnArrowCountChanged += OnArrowCountChanged;
        }

        // UI 초기화
        UpdateUI();

        // 타겟 생성 시작
        StartTargetSpawning();

        Debug.Log("활쏘기 시스템이 초기화되었습니다.");
    }

    /// <summary>
    /// 당김 강도 변경 이벤트 처리
    /// </summary>
    /// <param name="pullStrength">당김 강도 (0-1)</param>
    private void OnPullStrengthChanged(float pullStrength)
    {
        if (pullStrengthSlider != null)
        {
            pullStrengthSlider.value = pullStrength;
        }

        if (forceText != null)
        {
            float force = pullStrength * bowController.GetMaxPullDistance() * 25f; // 25는 발사 힘 배수
            forceText.text = $"발사 힘: {force:F1}";
        }
    }

    /// <summary>
    /// 화살 발사 이벤트 처리
    /// </summary>
    private void OnArrowReleased()
    {
        Debug.Log("화살이 발사되었습니다!");
        
        // 발사 효과나 사운드 추가 가능
        // 예: 화살 발사 파티클, 사운드 등
    }

    /// <summary>
    /// 화살 수 변경 이벤트 처리
    /// </summary>
    /// <param name="arrowCount">현재 화살 수</param>
    private void OnArrowCountChanged(int arrowCount)
    {
        UpdateArrowCountUI(arrowCount);
        
        // 화살이 부족하면 자동 보충
        if (autoRefillArrows && arrowCount <= 2)
        {
            StartCoroutine(RefillArrows());
        }
    }

    /// <summary>
    /// UI 업데이트
    /// </summary>
    private void UpdateUI()
    {
        UpdateArrowCountUI(bowController != null ? bowController.GetCurrentArrowCount() : 0);
        UpdateScoreUI();
    }

    /// <summary>
    /// 화살 수 UI 업데이트
    /// </summary>
    /// <param name="arrowCount">화살 수</param>
    private void UpdateArrowCountUI(int arrowCount)
    {
        if (arrowCountText != null)
        {
            arrowCountText.text = $"화살: {arrowCount}";
        }
    }

    /// <summary>
    /// 점수 UI 업데이트
    /// </summary>
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"점수: {currentScore}";
        }
    }

    /// <summary>
    /// 타겟 생성 시작
    /// </summary>
    private void StartTargetSpawning()
    {
        if (targetSpawnCoroutine != null)
        {
            StopCoroutine(targetSpawnCoroutine);
        }
        targetSpawnCoroutine = StartCoroutine(SpawnTargets());
    }

    /// <summary>
    /// 타겟 생성 코루틴
    /// </summary>
    private System.Collections.IEnumerator SpawnTargets()
    {
        while (true)
        {
            yield return new WaitForSeconds(targetSpawnInterval);
            
            if (targetPrefab != null && targetSpawnPoints != null && targetSpawnPoints.Length > 0)
            {
                SpawnRandomTarget();
            }
        }
    }

    /// <summary>
    /// 랜덤 위치에 타겟 생성
    /// </summary>
    private void SpawnRandomTarget()
    {
        int randomIndex = Random.Range(0, targetSpawnPoints.Length);
        Transform spawnPoint = targetSpawnPoints[randomIndex];
        
        if (spawnPoint != null)
        {
            GameObject target = Instantiate(targetPrefab, spawnPoint.position, spawnPoint.rotation);
            
            // 타겟에 필요한 컴포넌트 추가
            SetupTarget(target);
            
            Debug.Log($"타겟이 생성되었습니다: {spawnPoint.name}");
        }
    }

    /// <summary>
    /// 타겟 설정
    /// </summary>
    /// <param name="target">설정할 타겟 오브젝트</param>
    private void SetupTarget(GameObject target)
    {
        // HealthSystem 컴포넌트 추가
        HealthSystem healthSystem = target.GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            healthSystem = target.AddComponent<HealthSystem>();
        }

        // TargetController 컴포넌트 추가
        TargetController targetController = target.GetComponent<TargetController>();
        if (targetController == null)
        {
            targetController = target.AddComponent<TargetController>();
        }

        // 태그 설정
        target.tag = "Target";
    }

    /// <summary>
    /// 화살 자동 보충
    /// </summary>
    private System.Collections.IEnumerator RefillArrows()
    {
        yield return new WaitForSeconds(3f); // 3초 대기
        
        if (bowController != null)
        {
            // 수동으로 화살 생성
            bowController.CreateArrow();
            Debug.Log("화살이 보충되었습니다.");
        }
    }

    /// <summary>
    /// 점수 추가
    /// </summary>
    /// <param name="points">추가할 점수</param>
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreUI();
        
        if (scoreManager != null)
        {
            scoreManager.AddScore(points);
        }
    }

    /// <summary>
    /// 게임 재시작
    /// </summary>
    [ContextMenu("Restart Game")]
    public void RestartGame()
    {
        currentScore = 0;
        UpdateUI();
        
        // 기존 타겟들 제거
        GameObject[] existingTargets = GameObject.FindGameObjectsWithTag("Target");
        foreach (GameObject target in existingTargets)
        {
            Destroy(target);
        }
        
        // 타겟 생성 재시작
        StartTargetSpawning();
        
        Debug.Log("게임이 재시작되었습니다.");
    }

    /// <summary>
    /// 활쏘기 시스템 상태 확인
    /// </summary>
    [ContextMenu("Check System Status")]
    public void CheckSystemStatus()
    {
        Debug.Log("=== 활쏘기 시스템 상태 ===");
        Debug.Log($"활 컨트롤러: {(bowController != null ? "연결됨" : "연결 안됨")}");
        Debug.Log($"화살 장착: {(bowController != null ? bowController.IsArrowNocked() : false)}");
        Debug.Log($"현재 화살 수: {(bowController != null ? bowController.GetCurrentArrowCount() : 0)}");
        Debug.Log($"현재 점수: {currentScore}");
        Debug.Log($"ScoreManager: {(scoreManager != null ? "연결됨" : "연결 안됨")}");
        Debug.Log("========================");
    }

    void OnDestroy()
    {
        // 이벤트 리스너 제거
        if (bowController != null)
        {
            bowController.OnPullStrengthChanged -= OnPullStrengthChanged;
            bowController.OnArrowReleased -= OnArrowReleased;
            bowController.OnArrowCountChanged -= OnArrowCountChanged;
        }
    }
} 