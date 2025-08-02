using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Poison Bullet")]
public class PoisonBulletSkill : Skill
{
    public GameObject poisonBulletPrefab;

    public override void Execute(GameObject user, Vector3 target)
    {
        Vector3 direction = (target - user.transform.position).normalized;
        GameObject bullet = Instantiate(poisonBulletPrefab, user.transform.position, Quaternion.identity);

        PoisonBullet poisonBullet = bullet.GetComponent<PoisonBullet>();
        poisonBullet.SetDirection(direction);
    }
}
