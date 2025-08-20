using UnityEngine;

[System.Serializable]
public class SkillState
{
    public Skill skill;
    [HideInInspector]
    public float lastUseTime = -999f;

    public void SetSkill(Skill newSkill)
    {
        skill = newSkill; // Gán skill mới
        if (skill != null)
        {
            lastUseTime = -999f; // Reset thời gian để skill sẵn sàng sử dụng ngay
        }
    }

    public bool IsReady()
    {
        if (skill == null) return false; // Trả về false nếu skill chưa được gán
        return Time.time >= lastUseTime + skill.cooldown;
    }

    public void Use(GameObject user, Vector3 target)
    {
        if (skill != null && IsReady())
        {
            skill.Execute(user, target);
            lastUseTime = Time.time;
        }
    }
}