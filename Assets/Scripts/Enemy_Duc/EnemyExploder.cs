using UnityEngine;

public class EnemyExploder : Enemy
{
    public float explosionRadius = 3f;
    public int explosionDamage = 10;
    private bool exploded = false;

    protected override void DieEnemy()
    {
        isDead = true;
        animator.SetTrigger(Die);

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        float dieAnimLength = animator.GetCurrentAnimatorStateInfo(0).length;

        Invoke(nameof(ExplodeOnce), dieAnimLength / 2f);

        InvokeOnEnemyDeath();

        Destroy(gameObject, dieAnimLength);
    }

    private void ExplodeOnce()
    {
        if (exploded) return;
        exploded = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth health = hit.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(explosionDamage, transform);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
