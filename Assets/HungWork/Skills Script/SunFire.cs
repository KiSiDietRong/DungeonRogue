using UnityEngine;

public class SunFire : MonoBehaviour
{
    public float speed = 8f;
    public int instantDamage = 20;
    public int dotDamage = 5;
    public float dotDuration = 3f;
    public float lifetime = 3f;

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
