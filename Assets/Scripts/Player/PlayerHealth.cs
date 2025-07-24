using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float knockbackThrustAmount = 0.2f;
    [SerializeField] private float damageRecoveryTime = 1f;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private float archangelScytheRadius = 5f;

    private int currentHealth;
    private float armor;
    private bool canTakeDamage = true;
    private Knockback knockback;
    private Flash flash;
    private InventoryManager inventoryManager;
    private bool hasRevived = false; // Theo dõi xem đã hồi sinh chưa

    private void Awake()
    {
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found in scene!");
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        armor = 0;
        if (armorText != null)
        {
            armorText.gameObject.SetActive(false);
        }
        UpdateHealthUI();
    }

    public void TakeDamage(int damageAmount, Transform hitTransform)
    {
        if (!canTakeDamage || knockback.GettingKnockedBack) return;

        float remainingDamage = damageAmount;
        if (armor > 0)
        {
            float armorReduction = Mathf.Min(armor, damageAmount);
            armor -= armorReduction;
            remainingDamage -= armorReduction;
        }

        currentHealth = Mathf.Max(0, currentHealth - (int)remainingDamage);
        canTakeDamage = false;

        UpdateHealthUI();

        if (flash != null)
        {
            StartCoroutine(flash.FlashRoutine());
        }

        if (knockback != null)
        {
            knockback.GetKnockedBack(hitTransform, knockbackThrustAmount);
        }

        StartCoroutine(DamageRecoveryRoutine());

        if (currentHealth <= 0)
        {
            if (inventoryManager != null && inventoryManager.playerInventory.Exists(relic => relic.type == RelicType.SpiritShelter) && !hasRevived)
            {
                StartCoroutine(ReviveRoutine());
            }
            else
            {
                Die();
            }
        }
    }

    private IEnumerator ReviveRoutine()
    {
        hasRevived = true;
        currentHealth = maxHealth;
        armor = 0;
        UpdateHealthUI();
        canTakeDamage = false;

        inventoryManager.RemoveRelic(RelicType.SpiritShelter);
        Debug.Log("Player revived with full HP due to SpiritShelter. Relic removed.");

        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    public void Heal(int amount)
    {
        int totalHeal = amount;
        if (inventoryManager != null && inventoryManager.playerInventory.Exists(relic => relic.type == RelicType.JuicyOpal))
        {
            totalHeal += 1;
        }
        currentHealth = Mathf.Min(currentHealth + totalHeal, maxHealth);

        if (inventoryManager != null && inventoryManager.playerInventory.Exists(relic => relic.type == RelicType.ArchangelsScythe))
        {
            float scytheDamage = totalHeal * 4f;
            Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, archangelScytheRadius);
            foreach (Collider2D enemyCollider in nearbyEnemies)
            {
                if (enemyCollider.CompareTag("Enemy"))
                {
                    Enemy enemy = enemyCollider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(scytheDamage, enemy.transform.position, false);
                        Debug.Log($"ArchangelsScythe triggered: Dealt {scytheDamage} damage to {enemyCollider.name}.");
                    }
                }
            }
        }

        UpdateHealthUI();
    }

    public void AddArmor(float amount)
    {
        armor = Mathf.Min(armor + amount, 15f);
        UpdateHealthUI();
        Debug.Log($"Added {amount} Armor. Total Armor: {armor}");
    }

    public void InitFromStats(CharacterStatSO stats)
    {
        maxHealth = (int)stats.maxHealth;
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void SetArmorTextActive(bool active)
    {
        if (armorText != null)
        {
            armorText.gameObject.SetActive(active);
            if (active)
            {
                armorText.text = $"Armor: {armor}";
            }
        }
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }

        if (armorText != null && armorText.gameObject.activeSelf)
        {
            armorText.text = $"Armor: {armor}";
        }
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private void Die()
    {
        Debug.Log("Player has died!");
    }
}