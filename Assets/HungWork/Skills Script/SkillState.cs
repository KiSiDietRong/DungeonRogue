using UnityEngine;

[System.Serializable]
public class SkillState
{
    public Skill skill;
    [HideInInspector]
    public float lastUseTime = -999f;

    public void SetSkill(Skill newSkill)
    {
        skill = newSkill;
        if (skill != null)
        {
            lastUseTime = -999f; // reset => dùng ngay được
        }
    }

    public bool IsReady()
    {
        if (skill == null) return false;
        return Time.time >= lastUseTime + skill.cooldown;
    }

    // ✅ sửa: return bool để SkillManager biết có cast thành công không
    public bool Use(GameObject user, Vector3 target)
    {
        if (skill != null && IsReady())
        {
            skill.Execute(user, target);
            lastUseTime = Time.time;
            return true;
        }
        return false;
    }

    public float GetRemainingCooldown()
    {
        if (skill == null) return 0f;
        float remain = (lastUseTime + skill.cooldown) - Time.time;
        return Mathf.Max(remain, 0f);
    }
}
