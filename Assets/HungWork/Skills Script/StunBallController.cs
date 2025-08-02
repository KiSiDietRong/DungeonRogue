using UnityEngine;

public class StunBallController : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float stunDuration;
    private int damage;

    public void Initialize(Vector2 dir, float spd, float stunTime, int dmg)
    {
        direction = dir;
        speed = spd;
        stunDuration = stunTime;
        damage = dmg;

        Destroy(gameObject, 5f); // tự huỷ nếu không trúng gì
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                enemy.Stun(stunDuration);
            }

            Destroy(gameObject); // huỷ khi trúng kẻ địch
        }
    }
}
