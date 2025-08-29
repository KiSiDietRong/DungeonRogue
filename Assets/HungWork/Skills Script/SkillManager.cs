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
    private AudioSource playerAudioSource; // AudioSource trên nhân vật
    private SkillHudBinder hudBinder;

    void Start()
    {
        mainCamera = Camera.main;
        hudBinder = FindObjectOfType<SkillHudBinder>();

        // Lấy hoặc thêm AudioSource vào GameObject của nhân vật
        playerAudioSource = gameObject.GetComponent<AudioSource>();
        if (playerAudioSource == null)
        {
            playerAudioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("Added AudioSource to player GameObject");
        }
        // Cấu hình AudioSource cho SFX
        playerAudioSource.playOnAwake = false;
        playerAudioSource.loop = false;

        if (hudBinder == null)
        {
            Debug.LogError("SkillHudBinder not found!");
        }

        if (skillSlot1 == null || skillSlot2 == null)
        {
            Debug.LogWarning("Skill slots in SkillManager are not assigned!");
        }
    }

    public void SetSkill(SkillState newSkill, int slot)
    {
        if (slot == 1)
        {
            skillSlot1 = newSkill;
            Debug.Log($"SkillManager: Set slot1 to {(newSkill?.skill?.skillName ?? "null")}");
        }
        else if (slot == 2)
        {
            skillSlot2 = newSkill;
            Debug.Log($"SkillManager: Set slot2 to {(newSkill?.skill?.skillName ?? "null")}");
        }

        if (hudBinder != null)
        {
            hudBinder.Refresh();
            Debug.Log("Called SkillHudBinder.Refresh");
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
            if (skillSlot1 != null && skillSlot1.skill != null)
            {
                skillSlot1.Use(gameObject, GetTargetPosition(), playerAudioSource);
                TryActivateRelicEffects();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (skillSlot2 != null && skillSlot2.skill != null)
            {
                skillSlot2.Use(gameObject, GetTargetPosition(), playerAudioSource);
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
                    Debug.Log("Bob's Containment Field: 5-second shield activated.");
                }

                if (activeWeapon != null && inventoryManager.HasEmpoweredBangle())
                {
                    activeWeapon.ActivateDamageBoost(3f, 1.5f);
                    Debug.Log("Empowered Bangle: 50% damage boost for 3 seconds.");
                }

                hasUsedFirstSkillInRoom = true;
            }
        }

        InventoryManager inventory = FindObjectOfType<InventoryManager>();
        if (inventory != null && inventory.playerInventory.Exists(relic => relic.type == RelicType.FieryImbuement))
        {
            isFieryImbuementActive = true;
            fieryImbuementAttackCount = 7;
            Debug.Log("Fiery Imbuement: Next 7 attacks apply burn effect.");
        }
    }

    public void ResetFirstSkillUsage()
    {
        hasUsedFirstSkillInRoom = false;
        Debug.Log("Reset first skill usage.");
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
            Debug.Log($"Fiery Imbuement: {fieryImbuementAttackCount} attacks left.");
            if (fieryImbuementAttackCount <= 0)
            {
                isFieryImbuementActive = false;
                Debug.Log("Fiery Imbuement deactivated.");
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