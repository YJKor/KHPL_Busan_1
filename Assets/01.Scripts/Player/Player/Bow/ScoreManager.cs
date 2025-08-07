using UnityEngine;
using TMPro; // TextMeshPro 사용 시 필요

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // 싱글톤 패턴
    public TextMeshProUGUI scoreText; // UI 텍스트 참조
    private int score = 0; // 현재 점수

    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 유지 (선택사항)
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreText(); // 초기 점수 표시
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
}