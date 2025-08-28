using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [System.Serializable]
    public struct SpawnSetting
    {
        public GameObject enemyPrefab;
        public int amount; // số lượng enemy loại này
    }

    [System.Serializable]
    public class TurnSetting
    {
        public List<SpawnSetting> enemiesInTurn;
    }

    [Header("Enemy Settings")]
    public TurnSetting[] turnSettings;
    public float spawnDelay = 2f;
    public GameObject itemPrefab;
    public int maxTurns = 2;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Portal Settings")]
    public GameObject portalObject;

    [Header("Reward Settings")]
    public GameObject coinPrefab;
    public int rewardCoinAmount = 20;
    public float rewardScatterRadius = 3f;

    private List<GameObject> currentEnemies = new List<GameObject>();
    private bool isSpawning = false;
    private int currentTurn = 0;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (turnSettings == null || turnSettings.Length < maxTurns)
        {
            Debug.LogWarning("Chưa thiết lập đủ turnSettings cho số lượt.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Chưa thiết lập spawnPoints cho EnemySpawner");
            return;
        }

        SpawnNewTurn();
    }

    void Update()
    {
        if (currentEnemies.Count > 0 && currentEnemies.TrueForAll(e => e == null))
        {
            currentEnemies.Clear();
            currentTurn++;

            if (currentTurn < maxTurns && !isSpawning)
            {
                Invoke(nameof(SpawnNewTurn), spawnDelay);
                isSpawning = true;
            }
            else if (currentTurn >= maxTurns && !isSpawning)
            {
                SpawnItem();
                SpawnCoin();

                if (portalObject != null)
                    portalObject.SetActive(true);

                if (MapController.Instance != null)
                    MapController.Instance.StartCoroutine(MapController.Instance.ShowRoomComplete());
            }
        }
    }

    void SpawnNewTurn()
    {
        isSpawning = true;

        if (portalObject != null)
            portalObject.SetActive(false);

        var setting = turnSettings[currentTurn % turnSettings.Length];
        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);

        foreach (var enemySetting in setting.enemiesInTurn)
        {
            for (int i = 0; i < enemySetting.amount; i++)
            {
                if (availableSpawnPoints.Count == 0)
                {
                    Debug.LogWarning("Không còn đủ vị trí spawn cho số lượng enemy yêu cầu.");
                    break;
                }

                int randomIndex = Random.Range(0, availableSpawnPoints.Count);
                Transform spawnPoint = availableSpawnPoints[randomIndex];

                GameObject enemy = Instantiate(enemySetting.enemyPrefab, spawnPoint.position, Quaternion.identity);
                currentEnemies.Add(enemy);

                availableSpawnPoints.RemoveAt(randomIndex);
            }
        }

        isSpawning = false;
    }

    void SpawnItem()
    {
        if (itemPrefab == null)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 offset = Vector2.down;
            var playerScript = player.GetComponent<PlayerController>();
            if (playerScript != null && playerScript.lastMoveDirection != Vector2.zero)
                offset = playerScript.lastMoveDirection.normalized;

            Vector3 spawnPos = player.transform.position + (Vector3)(offset * 2f);
            Instantiate(itemPrefab, spawnPos, Quaternion.identity);
        }
    }

    void SpawnCoin()
    {
        if (coinPrefab == null)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 center = player.transform.position + Vector3.down * 1.5f;

            for (int i = 0; i < rewardCoinAmount; i++)
            {
                Vector2 randomOffset = Random.insideUnitCircle * rewardScatterRadius;
                Vector3 spawnPos = center + (Vector3)randomOffset;

                Quaternion randomRot = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

                GameObject coin = Instantiate(coinPrefab, spawnPos, randomRot);

                Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 pushDir = randomOffset.normalized * Random.Range(0.5f, 1.5f);
                    rb.linearVelocity = pushDir;
                }
            }
        }
    }

    public bool AllEnemiesCleared()
    {
        return currentEnemies.TrueForAll(e => e == null);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (spawnPoints != null)
        {
            foreach (var point in spawnPoints)
            {
                if (point != null)
                    Gizmos.DrawSphere(point.position, 0.2f);
            }
        }
    }
}