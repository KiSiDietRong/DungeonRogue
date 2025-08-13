using UnityEngine;
using System.Collections;

public class EnemySlowing : Enemy
{
    protected override IEnumerator AttackPlayer()
    {
        isAttacking = true;
        animator.SetTrigger(Attack);

        yield return new WaitForSeconds(attackTimeout);

        if (!isDead && !IsStunned)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance <= attackRange)
            {
                PlayerHealth health = player.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage((int)damage, transform);

                    // Gây làm chậm
                    PlayerController pc = player.GetComponent<PlayerController>();
                    if (pc != null)
                    {
                        pc.ApplySlow(slowTime); // bạn cần tạo hàm này trong PlayerController
                    }
                }
            }
        }

        animator.SetTrigger(Idle);
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }
}
