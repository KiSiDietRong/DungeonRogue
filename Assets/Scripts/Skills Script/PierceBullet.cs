using UnityEngine;

public class PierceBulletController : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float damage;
    private float lifeTime;
    private GameObject owner;

    private Rigidbody2D rb;

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
        transform.position += direction * speed * Time.deltaTime;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == owner) return;

        if (other.CompareTag("Enemy"))
        {
            if (hitTargets.Contains(other.gameObject)) return; 

            hitTargets.Add(other.gameObject);

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
