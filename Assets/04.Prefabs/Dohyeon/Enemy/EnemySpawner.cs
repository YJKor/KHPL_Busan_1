using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ������ �����ϰ� ���̺긦 �����ϴ� �ý���
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("������ ���� ������")]
    public GameObject enemyPrefab;

    [Tooltip("���� ����Ʈ��")]
    public Transform[] spawnPoints;

    [Tooltip("���̺� �� ��� �ð� (��)")]
    public float waveDelay = 5f;

    [Tooltip("���� ���� ���� (��)")]
    public float spawnInterval = 2f;

    [Header("Wave Settings")]
    [Tooltip("���� ���̺� ��ȣ")]
    public int currentWave = 1;

    [Tooltip("���̺�� ���� ��")]
    public int enemiesPerWave = 5;

    [Tooltip("���̺갡 ����ɼ��� ���� �� ������")]
    public int enemyIncreasePerWave = 2;

    [Tooltip("�ִ� ���̺� �� (0 = ����)")]
    public int maxWaves = 0;

    [Header("Enemy Types")]
    [Tooltip("�پ��� ���� �����յ�")]
    public GameObject[] enemyTypes;

    [Tooltip("�� ���� Ÿ���� ���� Ȯ�� (0~1)")]
    [Range(0f, 1f)]
    public float[] enemySpawnChances;

    [Header("Events")]
    [Tooltip("���̺갡 ���۵Ǿ��� �� ȣ��Ǵ� �̺�Ʈ")]
    public UnityEngine.Events.UnityEvent<int> OnWaveStart;

    [Tooltip("���̺갡 ������ �� ȣ��Ǵ� �̺�Ʈ")]
    public UnityEngine.Events.UnityEvent<int> OnWaveEnd;

    [Tooltip("��� ���̺갡 ������ �� ȣ��Ǵ� �̺�Ʈ")]
    public UnityEngine.Events.UnityEvent OnAllWavesComplete;

    // ���� ������
    private bool isSpawning = false;
    private int enemiesSpawned = 0;
    private int enemiesKilled = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();

    // ������Ƽ
    public bool IsSpawning => isSpawning;
    public int CurrentWave => currentWave;
    public int EnemiesSpawned => enemiesSpawned;
    public int EnemiesKilled => enemiesKilled;
    public int ActiveEnemies => activeEnemies.Count;

    void Start()
    {
        // ���� �������� �������� �ʾҴٸ� �⺻�� ����
        if (enemyPrefab == null && enemyTypes.Length > 0)
        {
            enemyPrefab = enemyTypes[0];
        }

        // ���� ����Ʈ�� �������� �ʾҴٸ� �ڵ����� ã��
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            FindSpawnPoints();
        }

        // ù ��° ���̺� ����
        StartCoroutine(StartWaveCoroutine());
    }

    /// <summary>
    /// ���� ����Ʈ���� �ڵ����� ã��
    /// </summary>
    void FindSpawnPoints()
    {
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (spawnPointObjects.Length > 0)
        {
            spawnPoints = new Transform[spawnPointObjects.Length];
            for (int i = 0; i < spawnPointObjects.Length; i++)
            {
                spawnPoints[i] = spawnPointObjects[i].transform;
            }
        }
        else
        {
            // �⺻ ���� ����Ʈ ����
            CreateDefaultSpawnPoints();
        }
    }

    /// <summary>
    /// �⺻ ���� ����Ʈ ����
    /// </summary>
    void CreateDefaultSpawnPoints()
    {
        spawnPoints = new Transform[3];
        for (int i = 0; i < 3; i++)
        {
            GameObject spawnPoint = new GameObject($"SpawnPoint_{i}");
            spawnPoint.tag = "SpawnPoint";
            spawnPoint.transform.position = transform.position + Vector3.forward * (10 + i * 5) + Vector3.right * (i - 1) * 3;
            spawnPoints[i] = spawnPoint.transform;
        }
    }

    /// <summary>
    /// ���̺� ����
    /// </summary>
    public void StartWave()
    {
        if (!isSpawning)
        {
            StartCoroutine(StartWaveCoroutine());
        }
    }

    /// <summary>
    /// ���̺� ���� �ڷ�ƾ
    /// </summary>
    IEnumerator StartWaveCoroutine()
    {
        isSpawning = true;
        enemiesSpawned = 0;
        enemiesKilled = 0;

        // ���̺� ���� �̺�Ʈ ȣ��
        OnWaveStart?.Invoke(currentWave);

        Debug.Log($"���̺� {currentWave} ����!");

        // ���� ���̺��� ���� �� ���
        int enemiesInThisWave = enemiesPerWave + (currentWave - 1) * enemyIncreasePerWave;

        // ���� ����
        for (int i = 0; i < enemiesInThisWave; i++)
        {
            SpawnEnemy();
            enemiesSpawned++;

            // ������ ������ �ƴϸ� ���
            if (i < enemiesInThisWave - 1)
            {
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        // ��� ������ ������ ��, ��� ������ �װų� ���� ������ ������ ���
        yield return new WaitUntil(() => activeEnemies.Count == 0);

        // ���̺� ����
        isSpawning = false;
        OnWaveEnd?.Invoke(currentWave);

        Debug.Log($"���̺� {currentWave} �Ϸ�! óġ�� ����: {enemiesKilled}");

        // ���� ���̺� �غ�
        currentWave++;

        // �ִ� ���̺� �� üũ
        if (maxWaves > 0 && currentWave > maxWaves)
        {
            OnAllWavesComplete?.Invoke();
            Debug.Log("��� ���̺� �Ϸ�!");
        }
        else
        {
            // ���� ���̺� ����
            yield return new WaitForSeconds(waveDelay);
            StartCoroutine(StartWaveCoroutine());
        }
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        // ���� ����Ʈ ����
        Transform spawnPoint = GetRandomSpawnPoint();
        if (spawnPoint == null) return;

        // ���� Ÿ�� ����
        GameObject enemyToSpawn = SelectEnemyType();

        // ���� ����
        GameObject enemy = Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);

        // ���� ��Ʈ�ѷ� ����
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            // �̺�Ʈ ����
            enemyController.OnEnemyDeath += OnEnemyDeath;
            enemyController.OnEnemyReachedCastle += OnEnemyReachedCastle;
        }

        // Ȱ�� ���� ��Ͽ� �߰�
        activeEnemies.Add(enemy);
    }

    /// <summary>
    /// ���� ���� ����Ʈ ����
    /// </summary>
    Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return null;
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    /// <summary>
    /// ���� Ÿ�� ����
    /// </summary>
    GameObject SelectEnemyType()
    {
        // �پ��� ���� Ÿ���� �ִٸ� Ȯ���� ���� ����
        if (enemyTypes.Length > 1 && enemySpawnChances.Length == enemyTypes.Length)
        {
            float random = Random.Range(0f, 1f);
            float cumulative = 0f;

            for (int i = 0; i < enemyTypes.Length; i++)
            {
                cumulative += enemySpawnChances[i];
                if (random <= cumulative)
                {
                    return enemyTypes[i];
                }
            }
        }

        // �⺻�� ��ȯ
        return enemyPrefab;
    }

    /// <summary>
    /// ������ �׾��� �� ȣ��
    /// </summary>
    void OnEnemyDeath(EnemyController enemy)
    {
        enemiesKilled++;
        activeEnemies.Remove(enemy.gameObject);
    }

    /// <summary>
    /// ������ ���� �������� �� ȣ��
    /// </summary>
    void OnEnemyReachedCastle(EnemyController enemy)
    {
        activeEnemies.Remove(enemy.gameObject);
    }

    /// <summary>
    /// ��� ���� ���� (����׿�)
    /// </summary>
    public void ClearAllEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        activeEnemies.Clear();
    }

    /// <summary>
    /// ���̺� ���� ����
    /// </summary>
    public void SetWaveSettings(int newEnemiesPerWave, float newSpawnInterval, float newWaveDelay)
    {
        enemiesPerWave = newEnemiesPerWave;
        spawnInterval = newSpawnInterval;
        waveDelay = newWaveDelay;
    }

    void OnDestroy()
    {
        // �̺�Ʈ ���� ����
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
            {
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.OnEnemyDeath -= OnEnemyDeath;
                    enemyController.OnEnemyReachedCastle -= OnEnemyReachedCastle;
                }
            }
        }
    }
}