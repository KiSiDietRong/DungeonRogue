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

    [Header("Spawn Area Boundaries")]
    public float maxX = 8f;
    public float maxY = 4f;

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

        for (int i = 0; i < enemiesPerTurn; i++)
        {
            Vector2 spawnPos = GetRandomSpawnPosition();
            GameObject enemy = Instantiate(currentEnemyPrefab, spawnPos, Quaternion.identity);
            currentEnemies.Add(enemy);
        }

        isSpawning = false;
    }

    void SpawnItem()
    {
        if (itemPrefab == null)
        {
            return;
        }

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

    Vector2 GetRandomSpawnPosition()
    {
        float x = Random.Range(-maxX, maxX);
        float y = Random.Range(-maxY, maxY);
        return new Vector2(x, y);
    }

    public bool AllEnemiesCleared()
    {
        // Chỉ trả về true khi không còn enemy sống
        return currentEnemies.TrueForAll(e => e == null);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 topLeft = new Vector3(-maxX, maxY, 0);
        Vector3 topRight = new Vector3(maxX, maxY, 0);
        Vector3 bottomLeft = new Vector3(-maxX, -maxY, 0);
        Vector3 bottomRight = new Vector3(maxX, -maxY, 0);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}