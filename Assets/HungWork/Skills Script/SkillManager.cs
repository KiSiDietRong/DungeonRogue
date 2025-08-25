using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public SkillState skillSlot1;
    public SkillState skillSlot2;
    public float maxCastDistance = 5f;

    [Header("UI Controllers")]
    public SkillUIController skillUIController1; // Gán trong Inspector
    public SkillUIController skillUIController2; // Gán trong Inspector

    private Camera mainCamera;
    private bool hasUsedFirstSkillInRoom = false;
    private bool isFieryImbuementActive = false;
    private int fieryImbuementAttackCount = 0;

    void Start()
    {
        mainCamera = Camera.main;
        // Cập nhật UI ban đầu
        if (skillSlot1 != null && skillUIController1 != null) skillUIController1.Setup(skillSlot1);
        if (skillSlot2 != null && skillUIController2 != null) skillUIController2.Setup(skillSlot2);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            if (skillSlot1 != null && skillSlot1.skill != null)
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

    // Gán skill mới và cập nhật UI
    public void AssignSkill(Skill newSkill, int slotIndex)
    {
        if (newSkill == null) return;

        if (slotIndex == 1 && skillSlot1 != null)
        {
            skillSlot1.SetSkill(newSkill);
            if (skillUIController1 != null) skillUIController1.Setup(skillSlot1);
        }
        else if (slotIndex == 2 && skillSlot2 != null)
        {
            skillSlot2.SetSkill(newSkill);
            if (skillUIController2 != null) skillUIController2.Setup(skillSlot2);
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
                }

                if (activeWeapon != null && inventoryManager.HasEmpoweredBangle())
                {
                    activeWeapon.ActivateDamageBoost(3f, 1.5f);
                }

                hasUsedFirstSkillInRoom = true;
            }
        }

        InventoryManager inventory = FindObjectOfType<InventoryManager>();
        if (inventory != null && inventory.playerInventory.Exists(relic => relic.type == RelicType.FieryImbuement))
        {
            isFieryImbuementActive = true;
            fieryImbuementAttackCount = 7;
        }
    }

    public void ResetFirstSkillUsage()
    {
        hasUsedFirstSkillInRoom = false;
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
            if (fieryImbuementAttackCount <= 0)
            {
                isFieryImbuementActive = false;
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
}