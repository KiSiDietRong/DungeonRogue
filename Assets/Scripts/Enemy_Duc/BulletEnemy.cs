using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    public float damage = 10f;
    public GameObject owner;
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
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

        if (other.gameObject == owner) return;

        //if (other.CompareTag("Obstacle"))
        //{
        //    Destroy(gameObject);
        //}
    }
}
