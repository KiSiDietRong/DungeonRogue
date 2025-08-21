using UnityEngine;

[System.Serializable]
public class SkillState
{
    public Skill skill;
    public float lastUseTime = -999f;

    public bool IsReady()
    {
        if (skill == null) return false;
        return Time.time >= lastUseTime + skill.cooldown;
    }

    public bool Use(GameObject user, Vector3 target)
    {
        if (!IsReady()) return false;
        if (skill == null) return false;

        skill.Execute(user, target);
        lastUseTime = Time.time;
        return true;
    }

    public float GetRemainingCooldown()
    {
        if (skill == null) return 0f;
        float remain = (lastUseTime + skill.cooldown) - Time.time;
        return Mathf.Max(remain, 0f);
    }
}