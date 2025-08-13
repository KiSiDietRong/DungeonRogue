using UnityEngine;
using System.Collections;

public class EnemySpawnerCocoon : Enemy
{
    [Header("Enemy Con")]
    public GameObject childEnemyPrefab; // Prefab enemy con
    public Transform spawnPoint; // Vị trí spawn
    public float spawnInterval = 1f; // Thời gian giữa mỗi lần spawn

    private bool isSpawning = true;

    protected override void Start()
    {
        base.Start(); // vẫn gọi Start của Enemy để setup máu, animator...

        if (spawnPoint == null)
            spawnPoint = transform;

        StartCoroutine(SpawnLoop());
    }

    // 🔹 Ghi đè Update để không di chuyển / tấn công
    protected override void Update()
    {
        // Cocoon không di chuyển hoặc tấn công
        // Có thể thêm animation Idle nếu muốn
    }

    private IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            SpawnChildEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnChildEnemy()
    {
        if (childEnemyPrefab != null)
        {
            Instantiate(childEnemyPrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    protected override void DieEnemy()
    {
        isSpawning = false; // ngừng spawn khi chết
        base.DieEnemy();
    }

    private void OnDestroy()
    {
        isSpawning = false;
    }
}
