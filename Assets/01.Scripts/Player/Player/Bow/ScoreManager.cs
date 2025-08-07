using UnityEngine;
using TMPro; // TextMeshPro ��� �� �ʿ�

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // �̱��� ����
    public TextMeshProUGUI scoreText; // UI �ؽ�Ʈ ����
    private int score = 0; // ���� ����

    void Awake()
    {
        // �̱��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� ���� (���û���)
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText(score);
    }

    void UpdateScoreText(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
}