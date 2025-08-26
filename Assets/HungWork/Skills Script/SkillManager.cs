using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("Skill Slots")]
    public SkillState skillSlot1;
    public SkillState skillSlot2;

    public float maxCastDistance = 5f;

    private Camera mainCamera;
    private bool hasUsedFirstSkillInRoom = false;
    private bool isFieryImbuementActive = false;
    private int fieryImbuementAttackCount = 0;

    private AudioSource audioSource;

    void Start()
    {
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (skillSlot1 == null || skillSlot2 == null)
        {
            Debug.LogWarning("Skill slots in SkillManager are not assigned. Please ensure they are initialized.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2)) // Middle mouse button
        {
            if (skillSlot1 != null && skillSlot1.skill != null)
            {
                skillSlot1.Use(gameObject, GetTargetPosition());
                PlaySound(skillSlot1.skill.castSound);
                TryActivateRelicEffects();
            }
        }

        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            if (skillSlot2 != null && skillSlot2.skill != null)
            {
                skillSlot2.Use(gameObject, GetTargetPosition());
                PlaySound(skillSlot2.skill.castSound);
                TryActivateRelicEffects();
            }
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
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
}
