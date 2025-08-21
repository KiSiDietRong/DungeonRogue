using UnityEngine;
using System.Collections;

public class EnemySuicideRanged : EnemyRanged
{
    [Header("Suicide Explosion Settings")]
    public float suicideDelay = 1f;     // Thời gian chạy trước khi nổ
    public float explosionRadius = 2f;  // Bán kính nổ
    public float explosionDamage = 10f; // Damage nổ
    public float speedMultiplier = 2f;  // Tăng tốc khi nổ

    private bool isSuiciding = false;

    protected override void DieEnemy()
    {
        if (isSuiciding) return; // tránh gọi 2 lần
        isSuiciding = true;

        // Không gọi base.DieEnemy ngay
        StartCoroutine(SuicideExplosion());
    }

    private IEnumerator SuicideExplosion()
    {
        // Tăng tốc độ
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

        // Play animation Die (giả sử là anim phát nổ)
        animator.SetTrigger(Die);

        // Gây sát thương AOE
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

        // Hủy enemy sau khi anim die chạy xong
        float dieAnimLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, dieAnimLength);
    }

    private void OnDrawGizmosSelected()
    {
        // Vẽ vùng nổ trong editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
