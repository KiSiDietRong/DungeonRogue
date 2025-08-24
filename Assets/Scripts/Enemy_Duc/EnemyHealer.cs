using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealer : Enemy
{
    [Header("Healing Settings")]
    public float healAmount = 10f;         // Lượng máu hồi mỗi lần
    public float healInterval = 2f;        // Thời gian giữa mỗi lần hồi
    public float healRange = 999f;         // Tầm hồi (999f = cả map)

    [Header("Effect Settings")]
    public GameObject healEffectPrefab;    // Prefab hiệu ứng hồi máu
    public Color healTextColor = Color.green; // Màu chữ hiển thị HP hồi

    private EnemyLineConnector lineConnector;

    protected override void Start()
    {
        currentHP = maxHP;
        player = GameObject.FindGameObjectWithTag("Player");
        if (animator == null) animator = GetComponent<Animator>();
        knockback = GetComponent<Knockback>();

        lineConnector = GetComponent<EnemyLineConnector>();

        StartCoroutine(HealLoop());
    }

    protected override void Update()
    {
        if (!isDead && animator != null)
        {
            animator.ResetTrigger(Walk);
            animator.SetTrigger(Idle);
        }
    }

    private IEnumerator HealLoop()
    {
        while (!isDead)
        {
            HealAllies();
            yield return new WaitForSeconds(healInterval);
        }
    }

    private void HealAllies()
    {
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        List<Transform> healedTargets = new List<Transform>();

        foreach (Enemy e in allEnemies)
        {
            if (e != this && !e.IsDead)
            {
                float dist = Vector2.Distance(transform.position, e.transform.position);
                if (dist <= healRange)
                {
                    e.Heal(healAmount);

                    if (healEffectPrefab != null)
                    {
                        GameObject effect = Instantiate(healEffectPrefab, e.transform.position, Quaternion.identity);
                        Destroy(effect, 1f);
                    }

                    healedTargets.Add(e.transform);

                    Debug.Log($"{gameObject.name} healed {e.gameObject.name} for {healAmount} HP");
                }
            }
        }

        // cập nhật dây nối
        if (lineConnector != null)
        {
            lineConnector.SetTargets(healedTargets);
        }
    }
}
