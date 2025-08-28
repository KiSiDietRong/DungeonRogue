using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public SkillState skillSlot1;
    public SkillState skillSlot2;

    public float maxCastDistance = 5f;

    private Camera mainCamera;
    private bool hasUsedFirstSkillInRoom = false;
    private bool isFieryImbuementActive = false;
    private int fieryImbuementAttackCount = 0;

    private SkillHudBinder hudBinder; // Tham chiếu tới SkillHudBinder

    void Start()
    {
        mainCamera = Camera.main;
        hudBinder = FindObjectOfType<SkillHudBinder>(); // Tìm SkillHudBinder
        if (hudBinder == null)
        {
            Debug.LogError("SkillHudBinder not found in scene!");
        }

        if (skillSlot1 == null || skillSlot2 == null)
        {
            Debug.LogWarning("Skill slots in SkillManager are not assigned. Please ensure they are initialized.");
        }
    }

    // Hàm để thay đổi skill trong runtime
    public void SetSkill(SkillState newSkill, int slot)
    {
        if (slot == 1)
        {
            skillSlot1 = newSkill;
            Debug.Log($"SkillManager: Set slot1 to {(newSkill?.skill?.name ?? "null")}");
        }
        else if (slot == 2)
        {
            skillSlot2 = newSkill;
            Debug.Log($"SkillManager: Set slot2 to {(newSkill?.skill?.name ?? "null")}");
        }

        // Gọi Refresh để cập nhật UI
        if (hudBinder != null)
        {
            hudBinder.Refresh();
        }
        else
        {
            Debug.LogError("Cannot refresh UI: SkillHudBinder not found!");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            if (skillSlot1 != null && skillSlot1.skill != null) // Đã sửa từ "スキルSlot1" thành "skillSlot1"
            {
                skillSlot1.Use(gameObject, GetTargetPosition());
                TryActivateRelicEffects();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (skillSlot2 != null && skillSlot2.skill != null)
            {
                skillSlot2.Use(gameObject, GetTargetPosition());
                TryActivateRelicEffects();
            }
        }
    }

    private void TryActivateRelicEffects()
    {
        if (!hasUsedFirstSkillInRoom)
        {
            InventoryManager inventoryManager = InventoryManager.Instance;
            PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
            ActiveWeapon activeWeapon = FindObjectOfType<ActiveWeapon>();

            if (inventoryManager != null)
            {
                if (playerHealth != null && inventoryManager.HasBobsContainmentField())
                {
                    playerHealth.ActivateShield(5f);
                    Debug.Log("Bob's Containment Field triggered: Activated 5-second shield on first skill use.");
                }

                if (activeWeapon != null && inventoryManager.HasEmpoweredBangle())
                {
                    activeWeapon.ActivateDamageBoost(3f, 1.5f);
                    Debug.Log("Empowered Bangle triggered: Increased weapon damage by 50% for 3 seconds on first skill use.");
                }

                hasUsedFirstSkillInRoom = true;
            }
        }

        InventoryManager inventory = FindObjectOfType<InventoryManager>();
        if (inventory != null && inventory.playerInventory.Exists(relic => relic.type == RelicType.FieryImbuement))
        {
            isFieryImbuementActive = true;
            fieryImbuementAttackCount = 7;
            Debug.Log("Fiery Imbuement triggered: Next 7 attacks will apply burn effect.");
        }
    }

    public void ResetFirstSkillUsage()
    {
        hasUsedFirstSkillInRoom = false;
        Debug.Log("Reset first skill usage for new room.");
    }

    public bool IsFieryImbuementActive()
    {
        return isFieryImbuementActive;
    }

    public void ConsumeFieryImbuementAttack()
    {
        if (isFieryImbuementActive)
        {
            fieryImbuementAttackCount--;
            Debug.Log($"Fiery Imbuement: {fieryImbuementAttackCount} attacks remaining.");
            if (fieryImbuementAttackCount <= 0)
            {
                isFieryImbuementActive = false;
                Debug.Log("Fiery Imbuement deactivated: No attacks remaining.");
            }
        }
    }

    Vector3 GetTargetPosition()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 playerPos = transform.position;
        Vector3 direction = mousePos - playerPos;
        float distance = direction.magnitude;

        if (distance <= maxCastDistance)
        {
            return mousePos;
        }
        else
        {
            return playerPos + direction.normalized * maxCastDistance;
        }
    }

    public void ResetRuntimeFlagsAndSlots()
    {
        hasUsedFirstSkillInRoom = false;
        isFieryImbuementActive = false;
        fieryImbuementAttackCount = 0;

        if (skillSlot1 != null) skillSlot1.SetSkill(null);
        if (skillSlot2 != null) skillSlot2.SetSkill(null);
    }
}