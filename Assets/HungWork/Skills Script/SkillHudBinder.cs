using UnityEngine;

public class SkillHudBinder : MonoBehaviour
{
    public SkillManager skillManager;
    public SkillUIController slot1UI;
    public SkillUIController slot2UI;

    void Start()
    {
        if (skillManager == null) skillManager = FindAnyObjectByType<SkillManager>();
        if (skillManager == null)
        {
            Debug.LogError("SkillManager not found in scene!");
            return;
        }

        Refresh(); // Khởi tạo UI ban đầu
    }

    public void Refresh()
    {
        Debug.Log("SkillHudBinder.Refresh called");
        if (skillManager == null)
        {
            Debug.LogError("skillManager is null in SkillHudBinder!");
            return;
        }

        if (slot1UI != null)
        {
            slot1UI.Setup(skillManager.skillSlot1);
            Debug.Log($"slot1UI updated with skill: {(skillManager.skillSlot1?.skill?.name ?? "null")}");
        }
        else
        {
            Debug.LogError("slot1UI is not assigned!");
        }

        if (slot2UI != null)
        {
            slot2UI.Setup(skillManager.skillSlot2);
            Debug.Log($"slot2UI updated with skill: {(skillManager.skillSlot2?.skill?.name ?? "null")}");
        }
        else
        {
            Debug.LogError("slot2UI is not assigned!");
        }
    }
}