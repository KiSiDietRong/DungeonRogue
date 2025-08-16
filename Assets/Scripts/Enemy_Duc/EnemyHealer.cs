using UnityEngine;
using System.Collections;

public class EnemyHealer : Enemy
{
    [Header("Healing Settings")]
    public float healAmount = 10f;         // Lượng máu hồi mỗi lần
    public float healInterval = 2f;        // Thời gian giữa mỗi lần hồi
    public float healRange = 999f;         // Tầm hồi (999f = cả map)

    [Header("Effect Settings")]
    public GameObject healEffectPrefab;    // Prefab hiệu ứng hồi máu
    public Color healTextColor = Color.green; // Màu chữ hiển thị HP hồi

    protected override void Start()
    {
        // Không gọi base.Start() để tránh patrol / chase
        currentHP = maxHP;
        player = GameObject.FindGameObjectWithTag("Player");
        if (animator == null) animator = GetComponent<Animator>();
        knockback = GetComponent<Knockback>();

        // Bắt đầu vòng lặp hồi máu
        StartCoroutine(HealLoop());
    }

    protected override void Update()
    {
        // Chỉ idle
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

        foreach (Enemy e in allEnemies)
        {
            if (e != this && !e.IsDead)
            {
                float dist = Vector2.Distance(transform.position, e.transform.position);
                if (dist <= healRange)
                {
                    e.Heal(healAmount);

                    // Spawn hiệu ứng hồi máu
                    if (healEffectPrefab != null)
                    {
                        GameObject effect = Instantiate(healEffectPrefab, e.transform.position, Quaternion.identity);
                        Destroy(effect, 1f); // Hủy sau 1 giây
                    }

                    // Hiển thị số HP hồi (nếu có hệ thống DamagePopup)
                    DamagePopup.Create(e.transform.position + Vector3.up * 1.2f, $"+{healAmount}", healTextColor);

                    Debug.Log($"{gameObject.name} healed {e.gameObject.name} for {healAmount} HP");
                }
            }
        }
    }
}
