using UnityEngine;

public abstract class Skill : ScriptableObject
{
    [Header("Basic Info")]
    public string skillName = "New Skill";
    public string skillInfo = "Skill Info";
    public Sprite icon;

    [Header("Cooldown")]
    public float cooldown = 2f;

    [Header("Sound")]
    public AudioClip castSound;

    public abstract void Execute(GameObject user, Vector3 target);
}
