using UnityEngine;

public class EnemyBerserker : Enemy
{
    [Header("Berserker Settings")]
    public float rageThreshold = 0.2f;   
    public float scaleMultiplier = 1.3f; 
    public float buffMultiplier = 1.5f;  

    private bool isEnraged = false;   

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

        transform.localScale *= scaleMultiplier;

        damage *= buffMultiplier;
        moveSpeed *= buffMultiplier;

        Debug.Log($"{gameObject.name} has entered Rage Mode! Damage and Speed increased.");
    }
}
