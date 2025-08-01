using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float speed = 10f;
    public float damageRadius = 2f;
    public int damage = 50;
    public string targetTag = "Enemy"; // Tag mục tiêu
    public GameObject explosionEffect;

    private Vector3 targetPosition;

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
    }

    void Update()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Explode();
        }
    }

    void Explode()
    {
        // Lấy toàn bộ Collider2D trong bán kính, không dùng layer
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, damageRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag(targetTag))
            {
                Enemy e = hit.GetComponent<Enemy>();
                if (e != null)
                {
                    e.TakeDamage(damage);
                }
            }
        }

        // FX nổ (nếu có)
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Tự hủy dù trúng hay không
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}