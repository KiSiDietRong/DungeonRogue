using UnityEngine;

[System.Serializable]
public class SkillState
{
    public Skill skill;
    [HideInInspector]
    public float lastUseTime = -999f;

    public bool IsReady()
    {
        return Time.time >= lastUseTime + skill.cooldown;
    }

    public void Use(GameObject user, Vector3 target)
    {
        if (IsReady())
        {
            skill.Execute(user, target);
            lastUseTime = Time.time;
        }
    }
}