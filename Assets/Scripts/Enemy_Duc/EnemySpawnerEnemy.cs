using UnityEngine;

public class EnemySpawnerEnemy : Enemy
{
    public GameObject miniEnemyPrefab;
    public int numberOfMinis = 3;

    protected override void DieEnemy()
    {
        base.DieEnemy();
        for (int i = 0; i < numberOfMinis; i++)
        {
            Vector3 spawnPos = transform.position + (Vector3)Random.insideUnitCircle * 0.5f;
            Instantiate(miniEnemyPrefab, spawnPos, Quaternion.identity);
        }
    }
}
