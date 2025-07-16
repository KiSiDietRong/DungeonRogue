using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float speed = 10f;
    public float damageRadius = 2f;
    public int damage = 50;
    public LayerMask enemyLayer;
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
        // Damage AOE
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, damageRadius, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyController e = enemy.GetComponent<EnemyController>();
            if (e != null)
            {
                e.TakeDamage(damage);
            }
        }

        // FX nổ (nếu có)
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Dù trúng hay không, vẫn tự huỷ
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}