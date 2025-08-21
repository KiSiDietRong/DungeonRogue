using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Following Skill")]
public class FollowingSkill : Skill
{
    public GameObject bulletPrefab;
    public float speed = 8f;
    public float damage = 10f;
    public float homingRange = 7f;
    public float lifeTime = 5f;

    public override void Execute(GameObject user, Vector3 target)
    {
        if (bulletPrefab == null) return;

        Vector3 spawnPos = user.transform.position;

        // 3 hướng cách đều nhau 120 độ
        float[] angles = { 0f, 120f, 240f };

        for (int i = 0; i < 3; i++)
        {
            float angleRad = angles[i] * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0);

            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

            float angleDeg = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angleDeg);

            FollowingBulletController controller = bullet.GetComponent<FollowingBulletController>();
            if (controller != null)
            {
                controller.Setup(direction, speed, damage, homingRange, lifeTime, user);
            }
        }
    }
}
