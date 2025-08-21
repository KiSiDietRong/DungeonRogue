using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Heal Allies")]
public class HealAlliesSkill : Skill
{
    public float healRadius = 5f;
    public int healAmount = 20;

    public override void Execute(GameObject user, Vector3 target)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(user.transform.position, healRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth health = hit.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.Heal(healAmount);
                }
            }
        }

        Debug.Log($"HealAlliesSkill: Healed nearby players within {healRadius} units for {healAmount} HP.");
    }
}
