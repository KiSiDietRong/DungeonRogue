using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Heal Self")]
public class HealSelfSkill : Skill
{
    public int healAmount = 30;

    public override void Execute(GameObject user, Vector3 target)
    {
        PlayerHealth playerHealth = user.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
            Debug.Log($"[HealSelfSkill] Healed player for {healAmount} HP.");
        }
        else
        {
            Debug.LogWarning("HealSelfSkill: No PlayerHealth found on user.");
        }
    }
}
