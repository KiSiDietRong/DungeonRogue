using UnityEngine;

public class SkillHudBinder : MonoBehaviour
{
    public SkillManager skillManager;       // script có skillSlot1, skillSlot2
    public SkillUIController slot1UI;       // tham chiếu tới SkillSlotUI 1
    public SkillUIController slot2UI;       // tham chiếu tới SkillSlotUI 2

    void Start()
    {
        if (skillManager == null) skillManager = FindAnyObjectByType<SkillManager>();

        if (slot1UI != null && skillManager != null)
            slot1UI.Setup(skillManager.skillSlot1);

        if (slot2UI != null && skillManager != null)
            slot2UI.Setup(skillManager.skillSlot2);
    }

    // Nếu game của bạn có hệ chọn/bật skill mới trong runtime,
    // có thể thêm hàm này để refresh UI khi đổi skill:
    public void Refresh()
    {
        if (slot1UI != null) slot1UI.Setup(skillManager.skillSlot1);
        if (slot2UI != null) slot2UI.Setup(skillManager.skillSlot2);
    }
}
