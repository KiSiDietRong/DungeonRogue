using UnityEngine;
using System.Collections;
using System.Linq;

public class EnemyPossessor : Enemy
{
    [Header("Possess Settings")]
    public float flySpeed = 5f;
    public float buffMultiplier = 1.1f;
    public Color buffedColor = Color.magenta;

    private bool isPossessing = false;

    protected override void DieEnemy()
    {
        if (isPossessing) return;
        isPossessing = true;

        if (isPossessed)
        {
            base.DieEnemy();
            return;
        }

        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        Enemy target = allEnemies
            .Where(e => e != this && !e.IsDead && !e.isPossessed) 
            .OrderBy(x => Random.value)
            .FirstOrDefault();

        if (target != null)
        {
            StartCoroutine(FlyToTargetAndPossess(target));
        }
        else
        {
            base.DieEnemy();
        }
    }

    private IEnumerator FlyToTargetAndPossess(Enemy target)
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        animator.ResetTrigger(Idle);
        animator.ResetTrigger(Walk);
        animator.SetTrigger(Die);

        while (target != null && !target.IsDead)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, flySpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, target.transform.position) < 0.2f)
            {
                ApplyBuff(target);
                break;
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    private void ApplyBuff(Enemy target)
    {
        target.maxHP *= buffMultiplier;
        target.damage *= buffMultiplier;
        target.moveSpeed *= buffMultiplier;
        target.Heal(target.maxHP * 0.1f);

        target.isPossessed = true;

        SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = buffedColor;
        }

        Debug.Log($"{gameObject.name} possessed {target.gameObject.name} → buffed stats!");
    }
}
