using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    private BossController boss;
    private bool isAlive = true;

    [Header("Attack Settings")]
    public float actionCooldown = 3f;  // thời gian nghỉ giữa các đòn
    private bool canAct = true;

    void Start()
    {
        boss = GetComponent<BossController>();
        StartCoroutine(AIBehaviour());
    }

    IEnumerator AIBehaviour()
    {
        while (isAlive)
        {
            if (canAct)
            {
                yield return new WaitForSeconds(1f); // chờ Idle 1 giây
                DecideAction();
                canAct = false;
                yield return new WaitForSeconds(actionCooldown);
                canAct = true;
            }
            yield return null;
        }
    }

    void DecideAction()
    {
        if (boss == null) return;

        float hpPercent = (float)boss.GetCurrentHealth() / boss.currentHP;
        int random = Random.Range(0, 100);

        // Phase 1: HP > 70%
        if (hpPercent > 0.7f)
        {
            if (random < 60) boss.DoShoot();
            else boss.DoMelee();
        }
        // Phase 2: HP 40% - 70%
        else if (hpPercent > 0.4f)
        {
            if (random < 40) boss.DoShoot();
            else if (random < 70) boss.DoMelee();
            else boss.DoShield();
        }
        // Phase 3: HP <= 40%
        else
        {
            if (random < 30) boss.DoShoot();
            else if (random < 60) boss.DoMelee();
            else if (random < 85) boss.DoShield();
            else boss.DoLaser();
        }
    }

    public void OnDeath()
    {
        isAlive = false;
        StopAllCoroutines();
    }
}
