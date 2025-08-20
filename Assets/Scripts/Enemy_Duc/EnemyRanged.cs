using UnityEngine;
using System.Collections;

public class EnemyRanged : Enemy
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float preferredDistance = 4f; // thêm khoảng cách lý tưởng

    protected override IEnumerator AttackPlayer()
    {
        isAttacking = true;
        animator.SetTrigger(Attack);

        yield return new WaitForSeconds(attackTimeout);

        if (!isDead && !IsStunned && projectilePrefab != null)
        {
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Vector2 direction = (player.transform.position - firePoint.position).normalized;
            bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * 6f;

            // Gán damage nếu có script Bullet
            BulletEnemy bulletScript = bullet.GetComponent<BulletEnemy>();
            if (bulletScript != null)
            {
                bulletScript.damage = damage;
                bulletScript.owner = gameObject; // để tránh tự gây sát thương nếu cần
            }
        }

        animator.SetTrigger(Idle);
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    protected override void MoveTowardsPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance < preferredDistance - 0.5f)
        {
            // Nếu quá gần thì lùi lại
            Vector2 dir = (transform.position - player.transform.position).normalized;
            transform.Translate(dir * moveSpeed * Time.deltaTime);
        }
        else if (distance > preferredDistance + 0.5f)
        {
            // Nếu quá xa thì tiến lại
            base.MoveTowardsPlayer();
        }
        else
        {
            // Đứng yên nếu trong khoảng an toàn
            animator.SetTrigger(Idle);
        }
    }
}
