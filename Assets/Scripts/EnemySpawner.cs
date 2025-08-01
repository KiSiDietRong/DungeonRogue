using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Enemy Settings")]
    public GameObject[] enemyPrefabs;
    public int enemiesPerTurn = 5;
    public float spawnDelay = 2f;
    public GameObject itemPrefab;
    public int maxTurns = 2;

    [Header("Spawn Points")]
    public Transform[] spawnPoints; // các điểm spawn cố định

    private List<GameObject> currentEnemies = new List<GameObject>();
    private bool isSpawning = false;
    private int currentTurn = 0;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length < maxTurns || System.Array.Exists(enemyPrefabs, prefab => prefab == null))
        {
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
            }
        }
    }

    void SpawnNewTurn()
    {
        isSpawning = true;

        GameObject currentEnemyPrefab = enemyPrefabs[currentTurn % enemyPrefabs.Length];

        // Tạo bản sao danh sách spawn points để tránh trùng lặp
        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);

        for (int i = 0; i < enemiesPerTurn; i++)
        {
            if (availableSpawnPoints.Count == 0)
            {
                Debug.LogWarning("Không còn đủ vị trí spawn cho số lượng enemy yêu cầu.");
                break;
            }

            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            Transform spawnPoint = availableSpawnPoints[randomIndex];

            GameObject enemy = Instantiate(currentEnemyPrefab, spawnPoint.position, Quaternion.identity);
            currentEnemies.Add(enemy);

            availableSpawnPoints.RemoveAt(randomIndex); // tránh trùng lặp
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
            //if (playerScript != null && playerScript.lastMoveDirection != Vector2.zero)
            //    offset = playerScript.lastMoveDirection.normalized;

            Vector3 spawnPos = player.transform.position + (Vector3)(offset * 2f);
            Instantiate(itemPrefab, spawnPos, Quaternion.identity);
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
