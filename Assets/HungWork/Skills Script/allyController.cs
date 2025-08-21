using UnityEngine;

public class allyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float attackRange = 1f;
    public int damage = 20;
    public float attackCooldown = 1f;
    public float TimeAvalable = 10f;
    public string enemyTag = "Enemy"; // <-- dùng tag thay vì layer

    private Transform targetEnemy;
    private float lastAttackTime = 0f;

    void Start()
    {
        Destroy(gameObject, TimeAvalable); // Tự hủy sau 5 giây
    }

    void Update()
    {
        FindClosestEnemy();

        if (targetEnemy != null)
        {
            float distance = Vector2.Distance(transform.position, targetEnemy.position);

            if (distance > attackRange)
            {
                Vector2 dir = (targetEnemy.position - transform.position).normalized;
                transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
            }
            else
            {
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
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag); // <-- Tìm bằng tag

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var enemyObj in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemyObj.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestEnemy = enemyObj.transform;
            }
        }

        targetEnemy = closestEnemy;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 10f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}