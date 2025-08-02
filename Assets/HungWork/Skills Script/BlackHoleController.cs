using UnityEngine;

public class BlackHoleController : MonoBehaviour
{
    public float pullForce = 5f;
    public float radius = 5f;
    public float duration = 3f;
    public string enemyTag = "Enemy"; // <-- chỉ hút enemy có tag này

    private float timer;

    void Start()
    {
        timer = duration;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        // Hút enemy trong vùng có tag đúng
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(enemyTag))
            {
                EnemyBlackHolePull enemy = hit.GetComponent<EnemyBlackHolePull>();
                if (enemy != null)
                {
                    enemy.PullTowards(transform.position, pullForce);
                }
            }
        }
    }

    void OnDestroy()
    {
        // Dừng hút khi skill biến mất
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(enemyTag))
            {
                EnemyBlackHolePull enemy = hit.GetComponent<EnemyBlackHolePull>();
                if (enemy != null)
                {
                    enemy.StopPull();
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}