using UnityEngine;

public class PoisonBullet : MonoBehaviour
{
    [Header("Cấu hình đạn")]
    public float speed = 10f;

    [Header("Hiệu ứng độc")]
    public float poisonDuration = 3f;
    public int poisonDamagePerSecond = 5;

    private Vector3 direction;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemypoison enemy = other.GetComponent<Enemypoison>();
            if (enemy != null)
            {
                enemy.ApplyPoison(poisonDuration, poisonDamagePerSecond);
            }

            Destroy(gameObject); // Tự hủy sau khi trúng
        }
    }
}
