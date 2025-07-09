using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// VR ���潺 ������ UI�� �����ϴ� ��ũ��Ʈ
/// </summary>
public class DefenseGameUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("���̺� ���� �ؽ�Ʈ")]
    public TextMeshProUGUI waveText;

    [Tooltip("�� ü�� ��")]
    public Slider castleHealthBar;

    [Tooltip("�� ü�� �ؽ�Ʈ")]
    public TextMeshProUGUI castleHealthText;

    [Tooltip("���� �ؽ�Ʈ")]
    public TextMeshProUGUI scoreText;

    [Tooltip("óġ�� ���� �� �ؽ�Ʈ")]
    public TextMeshProUGUI enemiesKilledText;

    [Tooltip("���� ���� �г�")]
    public GameObject gameOverPanel;

    [Tooltip("���� ���� �ؽ�Ʈ")]
    public TextMeshProUGUI gameOverText;

    [Tooltip("����� ��ư")]
    public Button restartButton;

    [Tooltip("���� �޴� ��ư")]
    public Button mainMenuButton;

    [Header("Game References")]
    [Tooltip("�� ü�� �ý���")]
    public CastleHealth castleHealth;

    [Tooltip("���� ���� �ý���")]
    public EnemySpawner enemySpawner;

    [Tooltip("���� ���� �ý���")]
    public ScoreManager scoreManager;

    // ���� ������
    private int currentScore = 0;
    private int enemiesKilled = 0;

    void Start()
    {
        InitializeUI();
        ConnectEvents();
    }

    /// <summary>
    /// UI �ʱ�ȭ
    /// </summary>
    void InitializeUI()
    {
        // ���� ���� �г� �����
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // ��ư �̺�Ʈ ����
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        // �ʱ� UI ������Ʈ
        UpdateWaveUI();
        UpdateScoreUI();
    }

    /// <summary>
    /// �̺�Ʈ ����
    /// </summary>
    void ConnectEvents()
    {
        //// �� ü�� �̺�Ʈ ����
        //if (castleHealth != null)
        //{
        //    castleHealth.OnHealthChanged += OnCastleHealthChanged;
        //    castleHealth.OnGameOver += OnGameOver;
        //}

        //// ���� ���� �̺�Ʈ ����
        //if (enemySpawner != null)
        //{
        //    enemySpawner.OnWaveStart += OnWaveStart;
        //    enemySpawner.OnWaveEnd += OnWaveEnd;
        //}

        //// ���� ���� �̺�Ʈ ����
        //if (scoreManager != null)
        //{
        //    // ScoreManager�� �̺�Ʈ�� �ִٸ� ����
        //    // scoreManager.OnScoreChanged += OnScoreChanged;
        //}
    }

    /// <summary>
    /// �� ü�� ���� �� ȣ��
    /// </summary>
    void OnCastleHealthChanged(int currentHealth, int maxHealth)
    {
        UpdateCastleHealthUI(currentHealth, maxHealth);
    }

    /// <summary>
    /// ���� ���� �� ȣ��
    /// </summary>
    void OnGameOver()
    {
        ShowGameOverUI();
    }

    /// <summary>
    /// ���̺� ���� �� ȣ��
    /// </summary>
    void OnWaveStart(int waveNumber)
    {
        UpdateWaveUI();
    }

    /// <summary>
    /// ���̺� ���� �� ȣ��
    /// </summary>
    void OnWaveEnd(int waveNumber)
    {
        UpdateWaveUI();
    }

    /// <summary>
    /// ���� ���� �� ȣ��
    /// </summary>
    void OnScoreChanged(int newScore)
    {
        currentScore = newScore;
        UpdateScoreUI();
    }

    /// <summary>
    /// ���̺� UI ������Ʈ
    /// </summary>
    void UpdateWaveUI()
    {
        if (waveText != null && enemySpawner != null)
        {
            waveText.text = $"���̺� {enemySpawner.CurrentWave}";
        }
    }

    /// <summary>
    /// �� ü�� UI ������Ʈ
    /// </summary>
    void UpdateCastleHealthUI(int currentHealth, int maxHealth)
    {
        if (castleHealthBar != null)
        {
            castleHealthBar.value = (float)currentHealth / maxHealth;
        }

        if (castleHealthText != null)
        {
            castleHealthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    /// <summary>
    /// ���� UI ������Ʈ
    /// </summary>
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"����: {currentScore}";
        }

        if (enemiesKilledText != null && enemySpawner != null)
        {
            enemiesKilledText.text = $"óġ: {enemySpawner.EnemiesKilled}";
        }
    }

    /// <summary>
    /// ���� ���� UI ǥ��
    /// </summary>
    void ShowGameOverUI()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverText != null)
        {
            gameOverText.text = $"���� ����!\n���� ����: {currentScore}\nóġ�� ����: {enemiesKilled}";
        }
    }

    /// <summary>
    /// ���� �����
    /// </summary>
    void RestartGame()
    {
        // �� ��ε�
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    /// <summary>
    /// ���� �޴��� �̵�
    /// </summary>
    void GoToMainMenu()
    {
        // ���� �޴� ������ �̵� (�� �̸��� ������Ʈ�� �°� ����)
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// ���� �߰�
    /// </summary>
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreUI();
    }

    /// <summary>
    /// óġ�� ���� �� ����
    /// </summary>
    public void AddEnemyKill()
    {
        enemiesKilled++;
        UpdateScoreUI();
    }

    void OnDestroy()
    {
        //// �̺�Ʈ ���� ����
        //if (castleHealth != null)
        //{
        //    castleHealth.OnHealthChanged -= OnCastleHealthChanged;
        //    castleHealth.OnGameOver -= OnGameOver;
        //}

        //if (enemySpawner != null)
        //{
        //    enemySpawner.OnWaveStart -= OnWaveStart;
        //    enemySpawner.OnWaveEnd -= OnWaveEnd;
        //}

        //if (restartButton != null)
        //{
        //    restartButton.onClick.RemoveListener(RestartGame);
        //}

        //if (mainMenuButton != null)
        //{
        //    mainMenuButton.onClick.RemoveListener(GoToMainMenu);
        //}
    }
}