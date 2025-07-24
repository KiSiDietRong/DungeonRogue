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
            }
            Destroy(gameObject);
        }
    }
}