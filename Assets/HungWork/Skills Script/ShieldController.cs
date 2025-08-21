using UnityEngine;

public class ShieldController : MonoBehaviour
{
    private float duration;
    private float timer;

    public void Setup(float lifeTime)
    {
        duration = lifeTime;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Nếu va chạm với đạn của kẻ địch
        if (other.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject); // Hủy đạn
        }
    }
}
