using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 22f;
    [SerializeField] private WeaponInfo weaponInfo;
    private Vector3 startPosition;
    private InventoryManager inventoryManager;

    private void Awake()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found in scene!");
        }
    }

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        MoveProjectile();
        DetectFireDistance();
    }

    public void UpdateWeaponInfo(WeaponInfo info)
    {
        weaponInfo = info;
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
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
                float finalDamage = weaponInfo.weaponDamage * damageMultiplier;
                enemy.TakeDamage(finalDamage, transform.position, isCritical);

                // Hồi HP nếu có RejuvenationGlove và đòn đánh là chí mạng
                if (isCritical && inventoryManager != null && inventoryManager.playerInventory.Exists(relic => relic.type == RelicType.RejuvenationGlove))
                {
                    PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.Heal(1);
                        Debug.Log("Rejuvenation Glove triggered: Player healed 1 HP on critical hit.");
                    }
                }

                // Gây sát thương sét ngẫu nhiên từ 3-15 nếu có VoltClaw và đòn đánh là chí mạng
                if (isCritical && inventoryManager != null && inventoryManager.playerInventory.Exists(relic => relic.type == RelicType.VoltClaw))
                {
                    float lightningDamage = Random.Range(3f, 15f);
                    enemy.TakeDamage(lightningDamage, transform.position, false);
                    Debug.Log($"VoltClaw triggered: Dealt {lightningDamage} lightning damage to {other.name}.");
                }

                Debug.Log($"Projectile dealt {finalDamage} damage to {other.name} (Critical: {isCritical}, RazorClaw: {damageMultiplier == 4f})");
            }
            Destroy(gameObject);
        }
    }
}