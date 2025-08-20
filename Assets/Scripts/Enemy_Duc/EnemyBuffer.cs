using UnityEngine;
using System.Collections.Generic;

public class EnemyBuffer : Enemy
{
    [Header("Buff Settings")]
    public float buffPercentage = 0.1f; // 10% buff
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
                // Buff chỉ số
                e.maxHP *= (1 + buffPercentage);
                e.damage *= (1 + buffPercentage);
                e.moveSpeed *= (1 + buffPercentage);

                // Đổi màu (nếu có SpriteRenderer)
                SpriteRenderer sr = e.GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = buffColor;

                // Thêm vào danh sách target để nối dây
                buffedTargets.Add(e.transform);
            }
        }

        // Cập nhật line connector
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
                // Trả lại chỉ số ban đầu (giảm 10%)
                e.maxHP /= (1 + buffPercentage);
                e.damage /= (1 + buffPercentage);
                e.moveSpeed /= (1 + buffPercentage);

                // Trả lại màu trắng mặc định
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

        // clear line khi chết
        if (lineConnector != null) lineConnector.SetTargets(new List<Transform>());

        base.DieEnemy();
    }
}
