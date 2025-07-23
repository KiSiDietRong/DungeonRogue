using UnityEngine;

public class BlackHoleController : MonoBehaviour
{
    public float pullForce = 5f;
    public float radius = 5f;
    public float duration = 3f;

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

        // Hút enemy trong vùng ảnh hưởng
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.isPulledByBlackHole = true;

                Vector3 dir = (transform.position - enemy.transform.position).normalized;
                enemy.transform.position += dir * pullForce * Time.deltaTime;
            }
        }
    }

    void OnDestroy()
    {
        // Khi hố đen biến mất, enemy trở lại bình thường
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.isPulledByBlackHole = false;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}