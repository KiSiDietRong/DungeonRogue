using UnityEngine;
using System.Collections;
using System.Linq;

public class EnemyPossessor : Enemy
{
    [Header("Possess Settings")]
    public float flySpeed = 5f;
    public float buffMultiplier = 1.1f; // +10%
    public Color buffedColor = Color.magenta;

    private bool isPossessing = false;

    protected override void DieEnemy()
    {
        if (isPossessing) return; // tránh gọi 2 lần
        isPossessing = true;

        // Tìm enemy khác còn sống
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        Enemy target = allEnemies
            .Where(e => e != this && !e.IsDead)
            .OrderBy(x => Random.value) // chọn random
            .FirstOrDefault();

        if (target != null)
        {
            StartCoroutine(FlyToTargetAndPossess(target));
        }
        else
        {
            // Nếu không có enemy nào để nhập -> chết bình thường
            base.DieEnemy();
        }
    }

    private IEnumerator FlyToTargetAndPossess(Enemy target)
    {
        // Disable collider để không va chạm
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        animator.ResetTrigger(Idle);
        animator.ResetTrigger(Walk);
        animator.SetTrigger(Die); // Play anim "hồn thoát" nếu có

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
        // Tăng chỉ số
        target.maxHP *= buffMultiplier;
        target.damage *= buffMultiplier;
        target.moveSpeed *= buffMultiplier;
        target.Heal(target.maxHP * 0.1f); // hồi thêm 10% máu ngay lập tức

        // Đổi màu sprite
        SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = buffedColor;
        }

        Debug.Log($"{gameObject.name} possessed {target.gameObject.name} → buffed stats!");
    }
}
