using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Pierce  Skill")]
public class PierceSkill : Skill
{
    public GameObject projectilePrefab;   // Prefab projectile
    public float projectileSpeed = 10f;
    public float projectileDamage = 20f;
    public float projectileLifeTime = 3f;

    public override void Execute(GameObject user, Vector3 target)
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPos = user.transform.position;
        Vector3 direction = (target - spawnPos).normalized;

        // Instantiate projectile
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        

        // Lấy controller và set tham số
        PierceBulletController controller = proj.GetComponent<PierceBulletController>();
        if (controller != null)
        {
            controller.Setup(direction, projectileSpeed, projectileDamage, projectileLifeTime, user);
        }
    }
}
