using System.Collections;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public IWeapon CurrentActiveWeapon { get; private set; }

    private GameObject currentWeaponObj;
    private float timeBetweenAttacks;
    private bool isAttacking = false;
    private int attackCount = 0;
    private InventoryManager inventoryManager;
    private bool isDamageBoosted = false;
    private float damageMultiplier = 1f;
    [SerializeField] private GameObject damageBoostEffectPrefab; // Prefab cho hiệu ứng tăng sát thương (tùy chọn)

    void Awake()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            Attack();
        }
    }

    public void SetActiveWeapon(GameObject newWeaponPrefab)
    {
        if (currentWeaponObj != null)
            Destroy(currentWeaponObj);

        currentWeaponObj = Instantiate(newWeaponPrefab, transform);
        CurrentActiveWeapon = currentWeaponObj.GetComponent<IWeapon>();

        timeBetweenAttacks = CurrentActiveWeapon.GetWeaponInfo().weaponCooldown;

        ClassChanger statHolder = currentWeaponObj.GetComponent<ClassChanger>();
        if (statHolder != null && statHolder.characterStat != null)
        {
            PlayerHealth playerHealth = GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.InitFromStats(statHolder.characterStat);
            }
        }
    }

    private void Attack()
    {
        if (CurrentActiveWeapon != null)
        {
            // Tạo bản sao WeaponInfo để áp dụng multiplier tạm thời
            WeaponInfo originalInfo = CurrentActiveWeapon.GetWeaponInfo();
            WeaponInfo modifiedInfo = ScriptableObject.CreateInstance<WeaponInfo>();
            modifiedInfo.name = originalInfo.name;
            modifiedInfo.weaponDamage = Mathf.RoundToInt(originalInfo.weaponDamage * (isDamageBoosted ? damageMultiplier : 1f)); // Ép kiểu float sang int
            modifiedInfo.criticalChance = originalInfo.criticalChance;
            modifiedInfo.weaponRange = originalInfo.weaponRange;
            modifiedInfo.weaponCooldown = originalInfo.weaponCooldown;
            modifiedInfo.weaponSprite = originalInfo.weaponSprite;

            CurrentActiveWeapon.Attack();
            attackCount++;

            if (inventoryManager != null && inventoryManager.playerInventory.Exists(relic => relic.type == RelicType.ConduitSpike) && attackCount >= 3)
            {
                ApplyConduitSpikeEffect();
                attackCount = 0;
            }

            StartCoroutine(AttackCooldownRoutine());
        }
    }

    public void ActivateDamageBoost(float duration, float multiplier)
    {
        if (!isDamageBoosted)
        {
            isDamageBoosted = true;
            damageMultiplier = multiplier;
            Debug.Log($"Damage boost activated: Weapon damage increased by {((multiplier - 1f) * 100)}% for {duration} seconds.");

            if (damageBoostEffectPrefab != null)
            {
                Vector3 spawnPosition = transform.position;
                GameObject boostEffect = Instantiate(damageBoostEffectPrefab, spawnPosition, Quaternion.identity, transform);
                Destroy(boostEffect, duration);
                Debug.Log("Damage boost effect instantiated and will be destroyed after duration.");
            }

            StartCoroutine(DeactivateDamageBoostAfterDelay(duration));
        }
    }

    private IEnumerator DeactivateDamageBoostAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        isDamageBoosted = false;
        damageMultiplier = 1f;
        Debug.Log("Damage boost deactivated.");
    }

    private void ApplyConduitSpikeEffect()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var enemiesByDistance = new System.Collections.Generic.List<(GameObject enemy, float distance)>();
        foreach (var enemy in enemies)
        {
            float distance = Vector2.Distance(player.transform.position, enemy.transform.position);
            enemiesByDistance.Add((enemy, distance));
        }

        enemiesByDistance.Sort((a, b) => a.distance.CompareTo(b.distance));

        int startIndex = enemiesByDistance.Count > 1 ? 1 : 0;
        int enemiesToDamage = Mathf.Min(2, enemiesByDistance.Count - startIndex);

        for (int i = startIndex; i < startIndex + enemiesToDamage; i++)
        {
            Enemy enemyScript = enemiesByDistance[i].enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                int damage = Random.Range(4, 11);
                enemyScript.TakeDamage(damage, enemyScript.transform.position, false);
            }
        }
    }

    private IEnumerator AttackCooldownRoutine()
    {
        isAttacking = true;
        yield return new WaitForSeconds(timeBetweenAttacks);
        isAttacking = false;
    }
}