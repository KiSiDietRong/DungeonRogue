using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUIController : MonoBehaviour
{
    [Header("UI Refs")]
    public Image iconImage;
    public Image cooldownMask;
    public TMP_Text cooldownText;

    private SkillState linkedSkill;

    public void Setup(SkillState state)
    {
        linkedSkill = state;

        if (linkedSkill == null || linkedSkill.skill == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Setup failed: SkillState or Skill is null");

            // Ẩn cả khung UI nếu không có skill
            gameObject.SetActive(false);
            return;
        }

        // Nếu có skill -> bật khung UI
        gameObject.SetActive(true);

        if (iconImage != null)
        {
            if (linkedSkill.skill.icon != null)
            {
                iconImage.sprite = linkedSkill.skill.icon;
                iconImage.enabled = true;
                Debug.Log($"[{gameObject.name}] Set icon for skill {linkedSkill.skill.name}");
            }
            else
            {
                iconImage.enabled = false;
                Debug.LogWarning($"[{gameObject.name}] Skill {linkedSkill.skill.name} has no icon sprite!");
            }
        }

        if (cooldownMask) cooldownMask.fillAmount = 0f;
        if (cooldownText) cooldownText.text = "";
    }

    void Update()
    {
        if (linkedSkill == null || linkedSkill.skill == null) return;

        float remain = linkedSkill.GetRemainingCooldown();
        float cd = linkedSkill.skill.cooldown;

        if (remain > 0f)
        {
            if (cooldownMask)
                cooldownMask.fillAmount = Mathf.Clamp01(remain / Mathf.Max(cd, 0.0001f));

            if (cooldownText)
                cooldownText.text = Mathf.CeilToInt(remain).ToString();
        }
        else
        {
            if (cooldownMask) cooldownMask.fillAmount = 0f;
            if (cooldownText) cooldownText.text = "";
        }
    }
}
