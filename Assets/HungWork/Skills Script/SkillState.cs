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
            lastUseTime = -999f;
            Debug.Log($"SkillState: Set skill to {newSkill.skillName}");
        }
    }

    public bool IsReady()
    {
        if (skill == null) return false;
        return Time.time >= lastUseTime + skill.cooldown;
    }

    public void Use(GameObject user, Vector3 target, AudioSource audioSource)
    {
        if (skill != null && IsReady())
        {
            skill.Execute(user, target);
            lastUseTime = Time.time;
            if (skill.castSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(skill.castSound);
                Debug.Log($"Playing sound for skill {skill.skillName}");
            }
            else
            {
                Debug.LogWarning($"No sound assigned for skill {skill.skillName} or AudioSource is null");
            }
        }
        else
        {
            Debug.Log($"Skill {skill?.skillName ?? "null"} on cooldown: {GetRemainingCooldown()}s left");
        }
    }

    public float GetRemainingCooldown()
    {
        if (skill == null) return 0f;
        float remain = (lastUseTime + skill.cooldown) - Time.time;
        return Mathf.Max(remain, 0f);
    }
}