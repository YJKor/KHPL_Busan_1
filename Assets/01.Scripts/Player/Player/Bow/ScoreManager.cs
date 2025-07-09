using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������ ���� �ý����� �����ϴ� ��ũ��Ʈ
/// ���� ����, ���̽��ھ�, UI ������Ʈ, ����/�ε� ����� �����մϴ�.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("���� ������ ǥ���� UI Text ������Ʈ")]
    public Text scoreText;

    [Tooltip("���̽��ھ ǥ���� UI Text ������Ʈ")]
    public Text highScoreText;

    [Header("Score Settings")]
    [Tooltip("���� ���ӿ��� ȹ���� ����")]
    public int currentScore = 0;

    [Tooltip("����� �ְ� ����")]
    public int highScore = 0;

    /// <summary>PlayerPrefs�� ������ ���̽��ھ��� Ű �̸�</summary>
    private const string HIGH_SCORE_KEY = "HighScore";

    /// <summary>
    /// ��ũ��Ʈ �ʱ�ȭ �� ȣ��Ǵ� �Լ�
    /// ����� ���̽��ھ �ε��ϰ� UI�� ������Ʈ�մϴ�.
    /// </summary>
    void Start()
    {
        LoadHighScore();
        UpdateScoreDisplay();
    }

    /// <summary>
    /// ������ �߰��ϴ� �Լ�
    /// ���� ������ ������ ������ ���ϰ� ���̽��ھ üũ�մϴ�.
    /// </summary>
    /// <param name="points">�߰��� ����</param>
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreDisplay();

        // ���̽��ھ� üũ �� ������Ʈ
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
            UpdateHighScoreDisplay();
        }
    }

    /// <summary>
    /// ���� ������ 0���� �����ϴ� �Լ�
    /// ���ο� ���� ���� �� ���˴ϴ�.
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreDisplay();
    }

    /// <summary>
    /// ���� ���� UI�� ������Ʈ�ϴ� �Լ�
    /// scoreText�� �����Ǿ� ���� ���� �۵��մϴ�.
    /// </summary>
    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }

    /// <summary>
    /// ���̽��ھ� UI�� ������Ʈ�ϴ� �Լ�
    /// highScoreText�� �����Ǿ� ���� ���� �۵��մϴ�.
    /// </summary>
    void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore.ToString();
        }
    }

    /// <summary>
    /// ���̽��ھ PlayerPrefs�� �����ϴ� �Լ�
    /// ���� ���� �ó� ���̽��ھ� ���� �� �ڵ����� ȣ��˴ϴ�.
    /// </summary>
    void SaveHighScore()
    {
        PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// PlayerPrefs���� ���̽��ھ �ε��ϴ� �Լ�
    /// ���� ���� �� �ڵ����� ȣ��˴ϴ�.
    /// </summary>
    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        UpdateHighScoreDisplay();
    }

    /// <summary>
    /// ���� ���� �� ȣ��Ǵ� �Լ�
    /// ���� ���̽��ھ �����մϴ�.
    /// </summary>
    public void GameOver()
    {
        SaveHighScore();
    }

    /// <summary>
    /// ���� ������ ��ȯ�ϴ� �Լ�
    /// �ٸ� ��ũ��Ʈ���� ���� ������ �ʿ��� �� ���˴ϴ�.
    /// </summary>
    /// <returns>���� ����</returns>
    public int GetCurrentScore()
    {
        return currentScore;
    }

    /// <summary>
    /// ���̽��ھ ��ȯ�ϴ� �Լ�
    /// �ٸ� ��ũ��Ʈ���� ���̽��ھ� ������ �ʿ��� �� ���˴ϴ�.
    /// </summary>
    /// <returns>���̽��ھ�</returns>
    public int GetHighScore()
    {
        return highScore;
    }

    /// <summary>
    /// ��� ���� �����͸� �ʱ�ȭ�ϴ� �Լ�
    /// ���̽��ھ���� ��� 0���� �����մϴ�.
    /// </summary>
    public void ResetAllScores()
    {
        currentScore = 0;
        highScore = 0;
        PlayerPrefs.DeleteKey(HIGH_SCORE_KEY);
        UpdateScoreDisplay();
        UpdateHighScoreDisplay();
    }
}