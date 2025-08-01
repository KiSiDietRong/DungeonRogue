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
    }

    private void Attack()
    {
        if (CurrentActiveWeapon != null)
        {
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