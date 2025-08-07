//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

///// <summary>
///// ScoreManager와 연동되는 UI 컴포넌트
///// 점수 표시, 연속 히트 표시, 애니메이션 효과를 담당합니다.
///// </summary>
//public class ScoreUI : MonoBehaviour
//{
//    [Header("UI References")]
//    [Tooltip("현재 점수를 표시할 Text 컴포넌트")]
//    [SerializeField] private Text scoreText;
    
//    [Tooltip("현재 점수를 표시할 TextMeshPro 컴포넌트")]
//    [SerializeField] private TextMeshProUGUI scoreTextTMP;
    
//    [Tooltip("최고 점수를 표시할 Text 컴포넌트")]
//    [SerializeField] private Text highScoreText;
    
//    [Tooltip("최고 점수를 표시할 TextMeshPro 컴포넌트")]
//    [SerializeField] private TextMeshProUGUI highScoreTextTMP;
    
//    [Tooltip("연속 히트를 표시할 Text 컴포넌트")]
//    [SerializeField] private Text consecutiveText;
    
//    [Tooltip("연속 히트를 표시할 TextMeshPro 컴포넌트")]
//    [SerializeField] private TextMeshProUGUI consecutiveTextTMP;

//    [Header("Animation")]
//    [Tooltip("점수 변경 시 애니메이션 효과")]
//    [SerializeField] private bool enableScoreAnimation = true;
    
//    [Tooltip("애니메이션 지속 시간")]
//    [SerializeField] private float animationDuration = 0.5f;
    
//    [Tooltip("애니메이션 스케일")]
//    [SerializeField] private float animationScale = 1.2f;

//    [Header("Colors")]
//    [Tooltip("연속 히트가 높을 때의 색상")]
//    [SerializeField] private Color highConsecutiveColor = Color.red;
    
//    [Tooltip("일반 색상")]
//    [SerializeField] private Color normalColor = Color.white;

//    private ScoreManager scoreManager;
//    private Vector3 originalScale;
//    private bool isAnimating = false;

//    void Start()
//    {
//        // ScoreManager 찾기
//        scoreManager = FindObjectOfType<ScoreManager>();
//        if (scoreManager == null)
//        {
//            Debug.LogWarning("ScoreManager를 찾을 수 없습니다!");
//            return;
//        }

//        // 이벤트 구독
//        scoreManager.OnScoreChanged += UpdateScoreDisplay;
//        scoreManager.OnHighScoreUpdated += UpdateHighScoreDisplay;

//        // 초기 UI 업데이트
//        UpdateScoreDisplay(scoreManager.GetCurrentScore());
//        UpdateHighScoreDisplay(scoreManager.GetHighScore());
//        UpdateConsecutiveDisplay(scoreManager.GetConsecutiveHits());

//        // 원본 크기 저장
//        if (scoreText != null)
//            originalScale = scoreText.transform.localScale;
//        else if (scoreTextTMP != null)
//            originalScale = scoreTextTMP.transform.localScale;
//    }

//    void OnDestroy()
//    {
//        // 이벤트 구독 해제
//        if (scoreManager != null)
//        {
//            scoreManager.OnScoreChanged -= UpdateScoreDisplay;
//            scoreManager.OnHighScoreUpdated -= UpdateHighScoreDisplay;
//        }
//    }

//    void Update()
//    {
//        // 연속 히트 실시간 업데이트
//        if (scoreManager != null)
//        {
//            UpdateConsecutiveDisplay(scoreManager.GetConsecutiveHits());
//        }
//    }

//    /// <summary>
//    /// 점수 표시 업데이트
//    /// </summary>
//    /// <param name="score">새로운 점수</param>
//    private void UpdateScoreDisplay(int score)
//    {
//        string scoreString = $"Score: {score:N0}";
        
//        // Text 컴포넌트 업데이트
//        if (scoreText != null)
//        {
//            scoreText.text = scoreString;
//            if (enableScoreAnimation && !isAnimating)
//            {
//                StartCoroutine(AnimateScore(scoreText.transform));
//            }
//        }
        
//        // TextMeshPro 컴포넌트 업데이트
//        if (scoreTextTMP != null)
//        {
//            scoreTextTMP.text = scoreString;
//            if (enableScoreAnimation && !isAnimating)
//            {
//                StartCoroutine(AnimateScore(scoreTextTMP.transform));
//            }
//        }
//    }

//    /// <summary>
//    /// 최고 점수 표시 업데이트
//    /// </summary>
//    /// <param name="highScore">새로운 최고 점수</param>
//    private void UpdateHighScoreDisplay(int highScore)
//    {
//        string highScoreString = $"High Score: {highScore:N0}";
        
//        if (highScoreText != null)
//            highScoreText.text = highScoreString;
        
//        if (highScoreTextTMP != null)
//            highScoreTextTMP.text = highScoreString;
//    }

//    /// <summary>
//    /// 연속 히트 표시 업데이트
//    /// </summary>
//    /// <param name="consecutiveHits">연속 히트 수</param>
//    private void UpdateConsecutiveDisplay(int consecutiveHits)
//    {
//        string consecutiveString = consecutiveHits > 1 ? $"Combo: x{consecutiveHits}" : "";
//        Color textColor = consecutiveHits > 3 ? highConsecutiveColor : normalColor;
        
//        // Text 컴포넌트 업데이트
//        if (consecutiveText != null)
//        {
//            consecutiveText.text = consecutiveString;
//            consecutiveText.color = textColor;
//        }
        
//        // TextMeshPro 컴포넌트 업데이트
//        if (consecutiveTextTMP != null)
//        {
//            consecutiveTextTMP.text = consecutiveString;
//            consecutiveTextTMP.color = textColor;
//        }
//    }

//    /// <summary>
//    /// 점수 변경 시 애니메이션 효과
//    /// </summary>
//    /// <param name="targetTransform">애니메이션할 Transform</param>
//    private System.Collections.IEnumerator AnimateScore(Transform targetTransform)
//    {
//        isAnimating = true;
//        Vector3 startScale = originalScale;
//        Vector3 endScale = originalScale * animationScale;
//        float elapsed = 0f;

//        // 확대
//        while (elapsed < animationDuration * 0.5f)
//        {
//            elapsed += Time.deltaTime;
//            float t = elapsed / (animationDuration * 0.5f);
//            targetTransform.localScale = Vector3.Lerp(startScale, endScale, t);
//            yield return null;
//        }

//        // 축소
//        elapsed = 0f;
//        while (elapsed < animationDuration * 0.5f)
//        {
//            elapsed += Time.deltaTime;
//            float t = elapsed / (animationDuration * 0.5f);
//            targetTransform.localScale = Vector3.Lerp(endScale, startScale, t);
//            yield return null;
//        }

//        targetTransform.localScale = originalScale;
//        isAnimating = false;
//    }

//    /// <summary>
//    /// 수동으로 점수 표시 업데이트 (외부에서 호출)
//    /// </summary>
//    public void ManualUpdate()
//    {
//        if (scoreManager != null)
//        {
//            UpdateScoreDisplay(scoreManager.GetCurrentScore());
//            UpdateHighScoreDisplay(scoreManager.GetHighScore());
//            UpdateConsecutiveDisplay(scoreManager.GetConsecutiveHits());
//        }
//    }

//    /// <summary>
//    /// 애니메이션 효과 토글
//    /// </summary>
//    public void ToggleAnimation()
//    {
//        enableScoreAnimation = !enableScoreAnimation;
//    }
//} 