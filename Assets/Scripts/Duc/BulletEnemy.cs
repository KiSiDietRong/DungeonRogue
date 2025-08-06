using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    public float damage = 10f;
    public GameObject owner;
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime); // tự huỷ sau X giây
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage((int)damage, transform);
            }
            Destroy(gameObject);
        }

        // Nếu cần tránh bắn trúng chính enemy
        if (other.gameObject == owner) return;

        // Nếu trúng tường hoặc vật cản
        if (other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
