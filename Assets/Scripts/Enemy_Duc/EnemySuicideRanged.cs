using UnityEngine;
using System.Collections;

public class EnemySuicideRanged : EnemyRanged
{
    [Header("Suicide Explosion Settings")]
    public float suicideDelay = 1f;     
    public float explosionRadius = 2f;  
    public float explosionDamage = 10f;
    public float speedMultiplier = 2f;  

    private bool isSuiciding = false;

    protected override void DieEnemy()
    {
        if (isSuiciding) return; 
        isSuiciding = true;

        StartCoroutine(SuicideExplosion());
    }

    private IEnumerator SuicideExplosion()
    {
        moveSpeed *= speedMultiplier;

        float timer = suicideDelay;
        while (timer > 0f)
        {
            if (player != null && !isDead)
            {
                Vector2 dir = (player.transform.position - transform.position).normalized;
                transform.Translate(dir * moveSpeed * Time.deltaTime);
            }
            timer -= Time.deltaTime;
            yield return null;
        }

        animator.SetTrigger(Die);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage((int)explosionDamage, transform);
                }
            }
        }

        float dieAnimLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, dieAnimLength);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
