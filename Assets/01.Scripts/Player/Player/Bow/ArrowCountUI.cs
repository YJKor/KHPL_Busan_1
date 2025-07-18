using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 화살 수를 UI에 표시하는 스크립트
/// </summary>
public class ArrowCountUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI arrowCountText;
    [SerializeField] private Image arrowIcon;
    [SerializeField] private Slider arrowProgressBar;
    
    [Header("Settings")]
    [SerializeField] private string arrowCountFormat = "화살: {0}";
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color lowArrowColor = Color.red;
    [SerializeField] private int lowArrowThreshold = 3;
    
    [Header("Animation")]
    [SerializeField] private bool enablePulseAnimation = true;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseScale = 1.2f;
    
    private BowController bowController;
    private int currentArrowCount = 0;
    private int maxArrowCount = 10;
    private Vector3 originalScale;
    private bool isPulsing = false;
    
    private void Start()
    {
        // BowController 찾기
        bowController = FindObjectOfType<BowController>();
        if (bowController != null)
        {
            // 이벤트 구독
            bowController.OnArrowCountChanged += UpdateArrowCount;
            bowController.OnArrowReleased += OnArrowReleased;
            
            // 초기 화살 수 설정
            maxArrowCount = bowController.GetMaxPullDistance() > 0 ? 10 : 10; // 기본값
            UpdateArrowCount(bowController.GetCurrentArrowCount());
        }
        
        // 원본 크기 저장
        if (arrowIcon != null)
        {
            originalScale = arrowIcon.transform.localScale;
        }
    }
    
    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (bowController != null)
        {
            bowController.OnArrowCountChanged -= UpdateArrowCount;
            bowController.OnArrowReleased -= OnArrowReleased;
        }
    }
    
    private void Update()
    {
        // 펄스 애니메이션
        if (enablePulseAnimation && isPulsing && arrowIcon != null)
        {
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * (pulseScale - 1f) * 0.5f;
            arrowIcon.transform.localScale = originalScale * pulse;
        }
    }
    
    /// <summary>
    /// 화살 수 업데이트
    /// </summary>
    /// <param name="count">현재 화살 수</param>
    private void UpdateArrowCount(int count)
    {
        currentArrowCount = count;
        
        // 텍스트 업데이트
        if (arrowCountText != null)
        {
            arrowCountText.text = string.Format(arrowCountFormat, count);
            
            // 화살 수가 적을 때 색상 변경
            arrowCountText.color = count <= lowArrowThreshold ? lowArrowColor : normalColor;
        }
        
        // 프로그레스 바 업데이트
        if (arrowProgressBar != null)
        {
            arrowProgressBar.value = (float)count / maxArrowCount;
        }
        
        // 아이콘 색상 업데이트
        if (arrowIcon != null)
        {
            arrowIcon.color = count <= lowArrowThreshold ? lowArrowColor : normalColor;
        }
        
        // 화살 수가 적을 때 펄스 애니메이션 시작
        if (count <= lowArrowThreshold && !isPulsing)
        {
            StartPulseAnimation();
        }
        else if (count > lowArrowThreshold && isPulsing)
        {
            StopPulseAnimation();
        }
    }
    
    /// <summary>
    /// 화살 발사 시 호출
    /// </summary>
    private void OnArrowReleased()
    {
        // 화살 발사 시 특별한 효과를 원한다면 여기에 추가
        if (enableDebugLogs)
            Debug.Log("화살이 발사되었습니다!");
    }
    
    /// <summary>
    /// 펄스 애니메이션 시작
    /// </summary>
    private void StartPulseAnimation()
    {
        isPulsing = true;
    }
    
    /// <summary>
    /// 펄스 애니메이션 중지
    /// </summary>
    private void StopPulseAnimation()
    {
        isPulsing = false;
        if (arrowIcon != null)
        {
            arrowIcon.transform.localScale = originalScale;
        }
    }
    
    /// <summary>
    /// 수동으로 화살 생성 (UI 버튼에서 호출)
    /// </summary>
    public void SpawnArrowButton()
    {
        if (bowController != null)
        {
            bowController.ManualSpawnArrow();
        }
    }
    
    /// <summary>
    /// 모든 화살 제거 (디버그용)
    /// </summary>
    public void ClearArrowsButton()
    {
        if (bowController != null)
        {
            bowController.ClearAllArrows();
        }
    }
    
    // 디버그용
    private bool enableDebugLogs = false;
} 