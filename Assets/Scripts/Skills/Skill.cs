using UnityEngine;

public abstract class Skill : ScriptableObject
{
    [Header("Basic Info")]
    public string skillName = "New Skill";
    public Sprite icon;

    [Header("Cooldown")]
    public float cooldown = 2f;

    public abstract void Execute(GameObject user, Vector3 target);
}