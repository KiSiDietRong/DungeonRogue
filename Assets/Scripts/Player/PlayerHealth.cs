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
    [SerializeField] private GameObject healEffectPrefab; 
    [SerializeField] private GameObject shieldEffectPrefab;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private TextMeshProUGUI timePlayedText;
    [SerializeField] private TextMeshProUGUI killedByText;
    [SerializeField] private TextMeshProUGUI enemyKilledText;
    [SerializeField] private TextMeshProUGUI damageDealtText;
    [SerializeField] private TextMeshProUGUI damageTakenText;

    private float playStartTime;
    private int totalDamageDealt = 0;
    private int totalDamageTaken = 0;
    private int totalEnemyKilled = 0;
    private string killedByEnemyName = "";

    private SpriteRenderer spriteRenderer;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private int currentHealth;
    private float armor;
    private bool canTakeDamage = true;
    private bool isShielded = false;
    private Knockback knockback;
    private Flash flash;
    private InventoryManager inventoryManager;
    private bool hasRevived = false;

    private void Awake()
    {
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found in scene!");
        }
        Enemy.OnEnemyDeath += OnEnemyDeathHandler;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        armor = 0;
        playStartTime = Time.time;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (armorText != null)
        {
            armorText.gameObject.SetActive(false);
        }
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }
        UpdateHealthUI();
    }

    public void TakeDamage(int damageAmount, Transform hitTransform)
    {
        if (!canTakeDamage || knockback.GettingKnockedBack || isShielded)
        {
            if (isShielded)
            {
                Debug.Log("Damage blocked by Bob's Containment Field shield.");
            }
            return;
        }

        float remainingDamage = damageAmount;
        if (armor > 0)
        {
            float armorReduction = Mathf.Min(armor, damageAmount);
            armor -= armorReduction;
            remainingDamage -= armorReduction;
        }

        totalDamageTaken += damageAmount;

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
            killedByEnemyName = hitTransform != null ? hitTransform.name : "Unknown Enemy";
            if (inventoryManager != null && inventoryManager.playerInventory.Exists(relic => relic.type == RelicType.SpiritShelter) && !hasRevived)
            {
                StartCoroutine(ReviveRoutine());
            }
            else
            {
                StartCoroutine(DieRoutine());
            }
        }
    }

    public void AddEnemyKilled() => totalEnemyKilled++;
    public void AddDamageDealt(int amount) => totalDamageDealt += amount;

    private IEnumerator DieRoutine()
    {
        Debug.Log("Player has died!");

        float duration = 1f;
        float elapsed = 0f;
        Color startColor = spriteRenderer.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        if (spriteRenderer != null) spriteRenderer.enabled = false;

        yield return new WaitForSecondsRealtime(0.2f);

        ShowGameOverCanvas();
    }

    private void ShowGameOverCanvas()
    {
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);

            float playTime = Time.time - playStartTime;
            System.TimeSpan ts = System.TimeSpan.FromSeconds(playTime);

            if (timePlayedText != null) timePlayedText.text = $"Run Time: {ts:mm\\:ss}";
            if (killedByText != null) killedByText.text = $"Defeated By: {killedByEnemyName}";
            if (enemyKilledText != null) enemyKilledText.text = $"{totalEnemyKilled}";
            if (damageDealtText != null) damageDealtText.text = $"{totalDamageDealt}";
            if (damageTakenText != null) damageTakenText.text = $"{totalDamageTaken}";
        }
        Time.timeScale = 0f;
    }

    public void ActivateShield(float duration)
    {
        if (!isShielded)
        {
            isShielded = true;
            Debug.Log($"Shield activated for {duration} seconds.");

            if (shieldEffectPrefab != null)
            {
                Vector3 spawnPosition = transform.position;
                GameObject shieldEffect = Instantiate(shieldEffectPrefab, spawnPosition, Quaternion.identity, transform);
                Destroy(shieldEffect, duration);
                Debug.Log("Shield effect instantiated and will be destroyed after duration.");
            }

            StartCoroutine(DeactivateShieldAfterDelay(duration));
        }
    }

    private IEnumerator DeactivateShieldAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        isShielded = false;
        Debug.Log("Shield deactivated.");
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

        if (healEffectPrefab != null)
        {
            Vector3 spawnPosition = transform.position + Vector3.up * 0.5f;
            GameObject healEffect = Instantiate(healEffectPrefab, spawnPosition, Quaternion.identity);
            Animator healAnimator = healEffect.GetComponent<Animator>();
            if (healAnimator != null)
            {
                healAnimator.Play("Heal", -1, 0f);
            }
            Destroy(healEffect, 1f);
            Debug.Log("Heal effect instantiated and will be destroyed after 1 second.");
        }

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
                        AddDamageDealt((int)scytheDamage);
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

    private void OnDestroy()
    {
        Enemy.OnEnemyDeath -= OnEnemyDeathHandler;
    }

    private void OnEnemyDeathHandler(Enemy enemy)
    {
        AddEnemyKilled();
    }

    public void ResetPlayer()
    {
        currentHealth = maxHealth;

        armor = 0;

        totalDamageDealt = 0;
        totalDamageTaken = 0;
        totalEnemyKilled = 0;
        killedByEnemyName = "";

        hasRevived = false;
        canTakeDamage = true;
        isShielded = false;

        playStartTime = Time.time;

        UpdateHealthUI();

        if (armorText != null)
        {
            armorText.gameObject.SetActive(false);
        }

        Debug.Log("Player state has been reset.");
    }
}