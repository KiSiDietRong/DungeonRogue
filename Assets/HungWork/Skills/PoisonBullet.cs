using UnityEngine;

public class PoisonBullet : MonoBehaviour
{
    public float speed = 6f;
    public int instantDamage = 10;
    public int dotDamage = 3;
    public float dotDuration = 4f;
    public float lifetime = 4f;

    private Vector2 direction;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(instantDamage);

                // Nếu đã có DOT → xóa để reset
                var existingDOT = collision.GetComponent<DamageOverTime>();
                if (existingDOT != null) Destroy(existingDOT);

                // Gắn DOT mới
                var dot = collision.gameObject.AddComponent<DamageOverTime>();
                dot.Init(dotDamage, dotDuration);
            }

            Destroy(gameObject);
        }
    }
}
