using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Score management system for the archery game
/// Handles score tracking, high score management, UI updates, and save/load functionality.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("UI Text component to display current score")]
    public Text scoreText;

    [Tooltip("UI Text component to display high score")]
    public Text highScoreText;

    [Tooltip("TextMeshPro component for current score (alternative to Text)")]
    public TextMeshProUGUI scoreTextTMP;

    [Tooltip("TextMeshPro component for high score (alternative to Text)")]
    public TextMeshProUGUI highScoreTextTMP;

    [Header("Score Settings")]
    [Tooltip("Points earned from hitting targets")]
    public int currentScore = 0;

    [Tooltip("Highest score achieved")]
    public int highScore = 0;

    [Tooltip("Points earned per target hit")]
    public int pointsPerHit = 10;

    [Tooltip("Bonus points for consecutive hits")]
    public int consecutiveHitBonus = 5;

    [Header("Events")]
    [Tooltip("Event triggered when score changes")]
    public System.Action<int> OnScoreChanged;

    [Tooltip("Event triggered when high score is updated")]
    public System.Action<int> OnHighScoreUpdated;

    /// <summary>PlayerPrefs key for storing high score</summary>
    private const string HIGH_SCORE_KEY = "HighScore";

    /// <summary>Consecutive hit counter for bonus points</summary>
    private int consecutiveHits = 0;

    /// <summary>
    /// Called when the script is initialized
    /// Loads the high score and updates the UI.
    /// </summary>
    void Start()
    {
        LoadHighScore();
        UpdateScoreDisplay();
    }

    /// <summary>
    /// Adds points to the current score
    /// Updates the score display and checks for new high score.
    /// </summary>
    /// <param name="points">Points to add</param>
    public void AddScore(int points)
    {
        if (points <= 0) return;

        currentScore += points;
        consecutiveHits++;
        
        // Add bonus for consecutive hits
        if (consecutiveHits > 1)
        {
            int bonus = consecutiveHitBonus * (consecutiveHits - 1);
            currentScore += bonus;
        }

        UpdateScoreDisplay();
        OnScoreChanged?.Invoke(currentScore);

        // Check and update high score
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
            UpdateHighScoreDisplay();
            OnHighScoreUpdated?.Invoke(highScore);
        }
    }

    /// <summary>
    /// Adds score for hitting a target
    /// </summary>
    public void OnTargetHit()
    {
        AddScore(pointsPerHit);
    }

    /// <summary>
    /// Resets consecutive hit counter when missing a target
    /// </summary>
    public void OnTargetMiss()
    {
        consecutiveHits = 0;
    }

    /// <summary>
    /// Resets the current score to 0
    /// Called when starting a new game.
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
        consecutiveHits = 0;
        UpdateScoreDisplay();
        OnScoreChanged?.Invoke(currentScore);
    }

    /// <summary>
    /// Updates the current score UI display
    /// Updates both Text and TextMeshPro components if available.
    /// </summary>
    void UpdateScoreDisplay()
    {
        string scoreString = "Score: " + currentScore.ToString();
        
        // Update legacy Text component
        if (scoreText != null)
        {
            scoreText.text = scoreString;
        }

        // Update TextMeshPro component
        if (scoreTextTMP != null)
        {
            scoreTextTMP.text = scoreString;
        }
    }

    /// <summary>
    /// Updates the high score UI display
    /// Updates both Text and TextMeshPro components if available.
    /// </summary>
    void UpdateHighScoreDisplay()
    {
        string highScoreString = "High Score: " + highScore.ToString();
        
        // Update legacy Text component
        if (highScoreText != null)
        {
            highScoreText.text = highScoreString;
        }

        // Update TextMeshPro component
        if (highScoreTextTMP != null)
        {
            highScoreTextTMP.text = highScoreString;
        }
    }

    /// <summary>
    /// Saves the high score to PlayerPrefs
    /// Called automatically when a new high score is achieved.
    /// </summary>
    void SaveHighScore()
    {
        PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the high score from PlayerPrefs
    /// Called automatically when the script starts.
    /// </summary>
    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        UpdateHighScoreDisplay();
    }

    /// <summary>
    /// Called when the game ends
    /// Saves the current high score.
    /// </summary>
    public void GameOver()
    {
        SaveHighScore();
    }

    /// <summary>
    /// Returns the current score
    /// Used by other scripts that need to access the current score.
    /// </summary>
    /// <returns>Current score</returns>
    public int GetCurrentScore()
    {
        return currentScore;
    }

    /// <summary>
    /// Returns the high score
    /// Used by other scripts that need to access the high score.
    /// </summary>
    /// <returns>High score</returns>
    public int GetHighScore()
    {
        return highScore;
    }

    /// <summary>
    /// Returns the consecutive hit count
    /// </summary>
    /// <returns>Number of consecutive hits</returns>
    public int GetConsecutiveHits()
    {
        return consecutiveHits;
    }

    /// <summary>
    /// Resets all score data to initial state
    /// Resets both current score and high score to 0.
    /// </summary>
    public void ResetAllScores()
    {
        currentScore = 0;
        highScore = 0;
        consecutiveHits = 0;
        PlayerPrefs.DeleteKey(HIGH_SCORE_KEY);
        UpdateScoreDisplay();
        UpdateHighScoreDisplay();
        OnScoreChanged?.Invoke(currentScore);
        OnHighScoreUpdated?.Invoke(highScore);
    }

    /// <summary>
    /// Sets the points earned per target hit
    /// </summary>
    /// <param name="points">Points per hit</param>
    public void SetPointsPerHit(int points)
    {
        pointsPerHit = Mathf.Max(1, points);
    }

    /// <summary>
    /// Sets the bonus points for consecutive hits
    /// </summary>
    /// <param name="bonus">Bonus points per consecutive hit</param>
    public void SetConsecutiveHitBonus(int bonus)
    {
        consecutiveHitBonus = Mathf.Max(0, bonus);
    }

    /// <summary>
    /// Debug function to test score system
    /// </summary>
    [ContextMenu("Test Add Score")]
    public void TestAddScore()
    {
        AddScore(10);
        Debug.Log($"Score added! Current score: {currentScore}, Consecutive hits: {consecutiveHits}");
    }

    /// <summary>
    /// Debug function to test target miss
    /// </summary>
    [ContextMenu("Test Target Miss")]
    public void TestTargetMiss()
    {
        OnTargetMiss();
        Debug.Log($"Target missed! Consecutive hits reset to: {consecutiveHits}");
    }
}