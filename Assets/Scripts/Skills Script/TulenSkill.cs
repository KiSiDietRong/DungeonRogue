using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Tulen")]
public class TulenSkill : Skill
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float projectileLifetime = 3f;
    public int damage = 10;
    public float spreadAngle = 15f; 

    public override void Execute(GameObject user, Vector3 target)
    {
        if (projectilePrefab == null) return;

        Vector3 direction = (target - user.transform.position).normalized;

        FireProjectile(user.transform.position, Quaternion.Euler(0, 0, 0) * direction); 
        FireProjectile(user.transform.position, Quaternion.Euler(0, 0, spreadAngle) * direction); 
        FireProjectile(user.transform.position, Quaternion.Euler(0, 0, -spreadAngle) * direction); 
    }

    private void FireProjectile(Vector3 startPos, Vector3 dir)
    {
        GameObject proj = Instantiate(projectilePrefab, startPos, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = dir.normalized * projectileSpeed;
        }

        TulenSkillController pc = proj.GetComponent<TulenSkillController>();
        if (pc != null)
        {
            pc.damage = damage;
        }

        Destroy(proj, projectileLifetime);
    }
}
