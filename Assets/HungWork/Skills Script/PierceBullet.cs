using UnityEngine;

public class PierceBulletController : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float damage;
    private float lifeTime;
    private GameObject owner;

    private Rigidbody2D rb;

    // Dùng để tránh đánh trùng địch nhiều lần
    private readonly System.Collections.Generic.HashSet<GameObject> hitTargets = new();

    public void Setup(Vector3 dir, float spd, float dmg, float lifetime, GameObject ownerObj)
    {
        direction = dir.normalized;
        speed = spd;
        damage = dmg;
        lifeTime = lifetime;
        owner = ownerObj;

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Di chuyển projectile
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Không đụng với owner hoặc projectile khác
        if (other.gameObject == owner) return;

        if (other.CompareTag("Enemy"))
        {
            if (hitTargets.Contains(other.gameObject)) return; // đã đánh rồi

            hitTargets.Add(other.gameObject);

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Không destroy để tiếp tục xuyên
        }

        // Nếu muốn dừng ở vật cản không xuyên, có thể check thêm tag "Obstacle" rồi Destroy(this.gameObject);
    }
}
