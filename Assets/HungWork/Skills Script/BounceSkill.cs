using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Bounce Skill")]
public class BounceSkill : Skill
{
    public GameObject bulletPrefab;
    public float speed = 10f;
    public float damage = 15f;
    public int maxBounces = 2;
    public float bounceRange = 5f;

    public override void Execute(GameObject user, Vector3 target)
    {
        if (bulletPrefab == null) return;

        Vector3 spawnPos = user.transform.position;
        Vector3 direction = (target - spawnPos).normalized;

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        BounceSkillController controller = bullet.GetComponent<BounceSkillController>();
        if (controller != null)
        {
            controller.Setup(direction, speed, damage, maxBounces, bounceRange, user);
        }
    }
}
