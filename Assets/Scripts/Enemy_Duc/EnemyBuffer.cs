using UnityEngine;
using System.Collections.Generic;

public class EnemyBuffer : Enemy
{
    [Header("Buff Settings")]
    public float buffPercentage = 0.1f; 
    public Color buffColor = Color.red;

    private bool isBuffing = false;
    private EnemyLineConnector lineConnector;

    protected override void Start()
    {
        currentHP = maxHP;
        player = GameObject.FindGameObjectWithTag("Player");
        if (animator == null) animator = GetComponent<Animator>();
        knockback = GetComponent<Knockback>();

        lineConnector = GetComponent<EnemyLineConnector>();

        BuffAllEnemies();
    }

    private void BuffAllEnemies()
    {
        if (isBuffing) return;

        List<Transform> buffedTargets = new List<Transform>();

        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        foreach (Enemy e in allEnemies)
        {
            if (e != this && !e.IsDead)
            {
                e.maxHP *= (1 + buffPercentage);
                e.damage *= (1 + buffPercentage);
                e.moveSpeed *= (1 + buffPercentage);

                SpriteRenderer sr = e.GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = buffColor;

                buffedTargets.Add(e.transform);
            }
        }

        if (lineConnector != null)
        {
            lineConnector.SetTargets(buffedTargets);
        }

        isBuffing = true;
    }

    private void RemoveBuffFromAllEnemies()
    {
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        foreach (Enemy e in allEnemies)
        {
            if (e != this && !e.IsDead)
            {
                e.maxHP /= (1 + buffPercentage);
                e.damage /= (1 + buffPercentage);
                e.moveSpeed /= (1 + buffPercentage);

                SpriteRenderer sr = e.GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = Color.white;
            }
        }
    }

    protected override void Update()
    {
        if (!isDead && animator != null)
        {
            animator.ResetTrigger(Walk);
            animator.SetTrigger(Idle);
        }
    }

    protected override void DieEnemy()
    {
        RemoveBuffFromAllEnemies();

        if (lineConnector != null) lineConnector.SetTargets(new List<Transform>());

        base.DieEnemy();
    }
}
