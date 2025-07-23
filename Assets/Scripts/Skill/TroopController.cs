using UnityEngine;

public class AllyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float attackRange = 1f;
    public int damage = 20;
    public float attackCooldown = 1.5f;

    public float lifeTime = 5f; // ? Ally ch? s?ng 5 giây

    public LayerMask enemyLayer;

    private Transform targetEnemy;
    private float lastAttackTime = 0f;

    void Start()
    {
        // T? h?y sau lifeTime giây
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        FindClosestEnemy();

        if (targetEnemy != null)
        {
            float distance = Vector2.Distance(transform.position, targetEnemy.position);

            if (distance > attackRange)
            {
                // Move towards enemy
                Vector2 dir = (targetEnemy.position - transform.position).normalized;
                transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
            }
            else
            {
                // Attack
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Enemy enemy = targetEnemy.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                        lastAttackTime = Time.time;
                    }
                }
            }
        }
    }

    void FindClosestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f, enemyLayer);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var enemyCollider in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemyCollider.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestEnemy = enemyCollider.transform;
            }
        }

        targetEnemy = closestEnemy;
    }

    // ? Có th? b? TakeDamage n?u k? d?ch chua b?n Ally
    public void TakeDamage(int dmg)
    {
        // N?u k? d?ch không t?n công Ally thì không c?n x? lý gì
        // B?n v?n có th? dùng sau này
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 10f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}