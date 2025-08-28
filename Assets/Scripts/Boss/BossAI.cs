using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    private BossController boss;
    private bool isAlive = true;

    private int phase = 1;
    private bool isInvulnerable = false;

    private int shootCount = 0;
    private float lastHP;
    private float shieldThreshold = 70f;
    private bool isShielding = false;

    void Start()
    {
        boss = GetComponent<BossController>();
        lastHP = boss.GetCurrentHealth();
        StartCoroutine(Phase1Routine());
    }

    private void Update()
    {
        if (!isAlive) return;

        float hpPercent = boss.GetCurrentHealth() / boss.maxHP;

        if (phase == 1 && hpPercent <= 0.6f)
        {
            phase = 2;
            StopAllCoroutines();
            StartCoroutine(EnterPhase2());
        }
        else if (phase == 2 && hpPercent <= 0.3f)
        {
            phase = 3;
            StopAllCoroutines();
            StartCoroutine(EnterPhase3());
        }

        if (phase == 3 && !isShielding)
        {
            if (lastHP - boss.GetCurrentHealth() >= shieldThreshold)
            {
                lastHP = boss.GetCurrentHealth();
                StopAllCoroutines();
                StartCoroutine(DoShieldThenResumePhase3());
            }
        }
    }

    IEnumerator Phase1Routine()
    {
        while (phase == 1 && isAlive)
        {
            boss.DoIdle();
            yield return new WaitForSeconds(1f);

            boss.DoShoot();

            boss.DoIdle();
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator EnterPhase2()
    {
        isInvulnerable = true;
        boss.DoGlow();
        yield return new WaitForSeconds(2f);
        isInvulnerable = false;

        StartCoroutine(Phase2Routine());
    }

    IEnumerator Phase2Routine()
    {
        while (phase == 2 && isAlive)
        {
            boss.DoIdle();
            yield return new WaitForSeconds(1f);

            boss.DoLaser();

            float laserDuration = 3f;
            yield return new WaitForSeconds(laserDuration + 1f);

            boss.DoIdle();
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator EnterPhase3()
    {
        isInvulnerable = true;
        boss.DoGlow();
        yield return new WaitForSeconds(2f);
        isInvulnerable = false;

        lastHP = boss.GetCurrentHealth();
        StartCoroutine(Phase3Routine());
    }

    IEnumerator Phase3Routine()
    {
        while (phase == 3 && isAlive)
        {
            boss.DoIdle();
            yield return new WaitForSeconds(1f);

            boss.DoShoot();
            shootCount++;

            boss.DoIdle();
            yield return new WaitForSeconds(2f);

            if (shootCount >= 3)
            {
                shootCount = 0;

                boss.DoIdle();
                yield return new WaitForSeconds(1f);

                boss.DoLaser();

                boss.DoIdle();
                yield return new WaitForSeconds(2f);
            }
        }
    }

    IEnumerator DoShieldThenResumePhase3()
    {
        isShielding = true;
        isInvulnerable = true;

        boss.DoShield();
        yield return new WaitForSeconds(3f);

        isInvulnerable = false;
        isShielding = false;

        StartCoroutine(Phase3Routine());
    }

    public void OnBossDamaged(float damage)
    {
        if (isShielding)
        {
            boss.currentHP = Mathf.Min(boss.maxHP, boss.currentHP + damage);
            BossHealthUI.Instance?.UpdateHealth(boss.currentHP);
        }
    }

    public void OnDeath()
    {
        isAlive = false;
        StopAllCoroutines();
    }

    public bool IsInvulnerable() => isInvulnerable;
}