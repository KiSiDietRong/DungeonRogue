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

    /// <summary>
    /// Gọi 1 lần khi bind skill vào ô UI.
    /// </summary>
    public void Setup(SkillState state)
    {
        linkedSkill = state;

        if (linkedSkill != null && linkedSkill.skill != null)
        {
            if (iconImage) iconImage.sprite = linkedSkill.skill.icon;
        }

        // clear UI
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
