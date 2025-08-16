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
        // ❌ Không gọi base.Start() để tránh PatrolLoop
        currentHP = maxHP;
        player = GameObject.FindGameObjectWithTag("Player");
        if (animator == null) animator = GetComponent<Animator>();
        knockback = GetComponent<Knockback>();

        if (spawnPoint == null)
            spawnPoint = transform;

        StartCoroutine(SpawnLoop());
    }

    protected override void Update()
    {
        // Chỉ Idle, không di chuyển hay tấn công
        if (!isDead && animator != null)
        {
            animator.SetTrigger(Idle);
        }
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
        isSpawning = false;
        base.DieEnemy();
    }

    private void OnDestroy()
    {
        isSpawning = false;
    }
}
