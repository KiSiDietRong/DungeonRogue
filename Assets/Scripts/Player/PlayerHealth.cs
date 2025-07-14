using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float knockbackThrustAmount = 10f;
    [SerializeField] private float damageRecoveryTime = 1f;

    private int currentHealth;
    private int maxHealth;

    private bool canTakeDamage = true;

    private Knockback knockback;
    private Flash flash;

    public CharacterStatSO characterStat;

    private void Awake()
    {
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
    }

    public void InitFromStats(CharacterStatSO stats)
    {
        characterStat = stats;
        maxHealth = Mathf.RoundToInt(characterStat.maxHealth);
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount, Transform hitTransform)
    {
        if (!canTakeDamage) return;

        canTakeDamage = false;
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
        }

    }
}
