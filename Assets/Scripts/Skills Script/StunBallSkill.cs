using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Stun Ball")]
public class StunBallSkill : Skill
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 8f;
    public float stunDuration = 2f;
    public int damage = 10;

    public override void Execute(GameObject user, Vector3 target)
    {
        Vector3 spawnPos = user.transform.position;
        Vector2 direction = (target - spawnPos).normalized;

        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        var ctrl = projectile.GetComponent<StunBallController>();
        if (ctrl != null)
        {
            ctrl.Initialize(direction, projectileSpeed, stunDuration, damage);
        }
    }
}
