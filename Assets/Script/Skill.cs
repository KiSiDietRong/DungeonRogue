using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public string skillName = "New Skill";
    public Sprite icon;
    public float cooldown = 2f;

    protected float lastUseTime = -999f;

    public bool IsReady()
    {
        return Time.time >= lastUseTime + cooldown;
    }

    public void Use(GameObject user, Vector3 target)
    {
        if (IsReady())
        {
            Execute(user, target);
            lastUseTime = Time.time;
        }
    }

    public abstract void Execute(GameObject user, Vector3 target);
}