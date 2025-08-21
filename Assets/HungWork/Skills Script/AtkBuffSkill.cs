using UnityEngine;

[CreateAssetMenu(menuName = "Skills/AtkBuffSkil")]
public class AtkBuffSkill : Skill
{
    public float buffDuration = 5f;
    public float damageMultiplier = 1.5f;

    public override void Execute(GameObject user, Vector3 target)
    {
        WeaponBuffController controller = user.GetComponent<WeaponBuffController>();
        if (controller != null)
        {
            controller.ApplyDamageBuff(damageMultiplier, buffDuration);
        }

        Debug.Log($"{skillName} used: Increased own damage x{damageMultiplier} for {buffDuration}s");
    }
}
