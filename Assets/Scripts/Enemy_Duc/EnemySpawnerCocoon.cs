using UnityEngine;
using System.Collections;

public class EnemySpawnerCocoon : Enemy
{
    [Header("Enemy Con")]
    public GameObject childEnemyPrefab; 
    public Transform spawnPoint; 
    public float spawnInterval = 1f; 

    private bool isSpawning = true;

    protected override void Start()
    {
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
