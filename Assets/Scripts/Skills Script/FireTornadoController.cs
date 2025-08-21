using UnityEngine;

public class FireTornadoController : MonoBehaviour
{
    private Transform player;
    private float angle;
    private float radius;
    private float speed;
    private float damage;
    private float duration;
    private float timer;

    public void Setup(Transform playerTransform, float startAngle, float orbitRadius, float orbitSpeed, float dmg, float lifeTime)
    {
        player = playerTransform;
        angle = startAngle;
        radius = orbitRadius;
        speed = orbitSpeed;
        damage = dmg;
        duration = lifeTime;
    }

    void Update()
    {
        if (player == null)
        {
            Destroy(gameObject);
            return;
        }

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Destroy(gameObject);
            return;
        }

        angle += speed * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
        transform.position = player.position + offset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
