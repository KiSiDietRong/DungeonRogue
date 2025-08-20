using UnityEngine;

public class EnemyBerserker : Enemy
{
    [Header("Berserker Settings")]
    public float rageThreshold = 0.2f;   // 20% máu
    public float scaleMultiplier = 1.3f; // Tăng scale
    public float buffMultiplier = 1.5f;  // +50% dmg, speed

    private bool isEnraged = false;      // Chỉ kích hoạt 1 lần

    protected override void Update()
    {
        base.Update();

        if (!isEnraged && currentHP <= maxHP * rageThreshold)
        {
            EnterRageMode();
        }
    }

    private void EnterRageMode()
    {
        isEnraged = true;

        // Tăng kích thước
        transform.localScale *= scaleMultiplier;

        // Buff dmg + speed
        damage *= buffMultiplier;
        moveSpeed *= buffMultiplier;

        Debug.Log($"{gameObject.name} has entered Rage Mode! Damage and Speed increased.");
    }
}
