using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 22f;
    [SerializeField] private WeaponInfo weaponInfo;
    private Vector3 startPosition;
    private InventoryManager inventoryManager;
    private bool isDamageBoosted = false;
    private float damageBoostEndTime = 0f;
    [SerializeField] private GameObject damageBoostEffectPrefab;

    private PlayerHealth playerHealth;

    private void Awake()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        playerHealth = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerHealth>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found in scene!");
        }
    }

    private void Start()
    {
        startPosition = transform.position;

        ActiveWeapon activeWeapon = FindObjectOfType<ActiveWeapon>();
        if (activeWeapon != null && activeWeapon.IsDamageBoosted)
        {
            ActivateDamageBoost(3f);
        }
    }

    private void Update()
    {
        MoveProjectile();
        DetectFireDistance();
        if (isDamageBoosted && Time.time > damageBoostEndTime)
        {
            isDamageBoosted = false;
            Debug.Log("Empowered Bangle damage boost deactivated.");
        }
    }

    public void UpdateWeaponInfo(WeaponInfo info)
    {
        weaponInfo = info;
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }

    public void ActivateDamageBoost(float duration)
    {
        if (!isDamageBoosted)
        {
            isDamageBoosted = true;
            damageBoostEndTime = Time.time + duration;
            Debug.Log($"Damage boost activated for {duration} seconds.");

            if (damageBoostEffectPrefab != null)
            {
                Vector3 spawnPosition = transform.position;
                GameObject boostEffect = Instantiate(damageBoostEffectPrefab, spawnPosition, Quaternion.identity, transform);
                Destroy(boostEffect, duration);
                Debug.Log("Damage boost effect instantiated and will be destroyed after duration.");
            }
        }
    }

    private void DetectFireDistance()
    {
        if (Vector3.Distance(transform.position, startPosition) > weaponInfo.weaponRange)
        {
            Destroy(gameObject);
        }
    }

    private void MoveProjectile()
    {
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && weaponInfo != null)
            {
                bool isCritical = Random.value <= weaponInfo.criticalChance;
                float damageMultiplier = isCritical ?
                    (inventoryManager != null && inventoryManager.playerInventory.Exists(relic => relic.type == RelicType.RazorClaw) ? 4f : 2f) : 1f;

                if (isDamageBoosted && Time.time <= damageBoostEndTime)
                {
                    damageMultiplier *= 1.5f;
                    Debug.Log($"Empowered Bangle triggered: Increased damage by 50% to {weaponInfo.weaponDamage * damageMultiplier}.");
                }

                float finalDamage = weaponInfo.weaponDamage * damageMultiplier;

                bool isStunned = enemy.IsStunned;
                if (isStunned && inventoryManager != null && inventoryManager.playerInventory.Exists(relic => relic.type == RelicType.GiantMace))
                {
                    finalDamage *= 1.5f;
                    Debug.Log($"GiantMace triggered: Increased damage by 50% to {finalDamage} on stunned enemy {other.name}.");
                }

                if (playerHealth != null)
                {
                    playerHealth.AddDamageDealt((int)finalDamage);
                }

                enemy.TakeDamage(finalDamage, transform.position, isCritical);

                if (isCritical && inventoryManager != null && inventoryManager.playerInventory.Exists(relic => relic.type == RelicType.RejuvenationGlove))
                {
                    PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.Heal(1);
                        Debug.Log("Rejuvenation Glove triggered: Player healed 1 HP on critical hit.");
                    }
                }

                if (isCritical && inventoryManager != null && inventoryManager.playerInventory.Exists(relic => relic.type == RelicType.VoltClaw))
                {
                    float lightningDamage = Random.Range(3f, 15f);
                    enemy.TakeDamage(lightningDamage, transform.position, false);
                    if (playerHealth != null)
                    {
                        playerHealth.AddDamageDealt((int)lightningDamage);
                    }
                    Debug.Log($"VoltClaw triggered: Dealt {lightningDamage} lightning damage to {other.name}.");
                }

                if (isCritical && inventoryManager != null && inventoryManager.playerInventory.Exists(relic => relic.type == RelicType.DazeClaw))
                {
                    enemy.Stun(1f);
                    Debug.Log($"DazeClaw triggered: Stunned {other.name} for 1 second.");
                }

                SkillManager skillManager = FindObjectOfType<SkillManager>();
                if (skillManager != null && skillManager.IsFieryImbuementActive())
                {
                    enemy.ApplyBurnEffect();
                    skillManager.ConsumeFieryImbuementAttack();
                    Debug.Log($"Fiery Imbuement: Applied burn effect to {other.name}.");
                }
            }
            Destroy(gameObject);
        }
    }
}