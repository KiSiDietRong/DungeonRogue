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
            Enemy enemy = hit.GetComponent<Enemy>();
            //if (enemy != null && enemy.moveSpeed > 0)
            //{
            //    enemy.PullTowards(transform.position);
            //}
        }
    }

    void OnDestroy()
    {
        // Ngừng hút enemy trong vùng khi Black Hole biến mất
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            //if (enemy != null)
            //{
            //    enemy.StopPull();
            //}
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}