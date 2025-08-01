using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Dash Skill")]
public class Dash : Skill
{
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;

    public override void Execute(GameObject user, Vector3 target)
    {
        var dashController = user.GetComponent<DashController>();
        if (dashController != null)
        {
            Vector2 dashDir = (target - user.transform.position).normalized;
            dashController.PerformDash(dashDir, dashDistance, dashDuration);
        }
        else
        {
            Debug.LogWarning("DashController is missing on player.");
        }
    }
}