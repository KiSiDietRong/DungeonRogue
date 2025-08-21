using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Triple Boomerang Skill")]
public class TripleBoomerangSkill : Skill
{
    [Header("Boomerang Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
    public float maxDistance = 5f;
    public float returnSpeed = 6f;
    public float sideAngle = 20f; // góc lệch 2 tia bên ngoài
    public int damage = 10;
    public string targetTag = "Enemy";

    public override void Execute(GameObject user, Vector3 target)
    {
        Vector2 direction = (target - user.transform.position).normalized;
        FireBoomerang(user, direction, 0f); // tia giữa
        FireBoomerang(user, direction, sideAngle); // tia lệch trái
        FireBoomerang(user, direction, -sideAngle); // tia lệch phải
    }

    private void FireBoomerang(GameObject user, Vector2 direction, float angleOffset)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, angleOffset);
        Vector2 newDir = rotation * direction;

        GameObject proj = Instantiate(projectilePrefab, user.transform.position, Quaternion.identity);
        BoomerangController ctrl = proj.GetComponent<BoomerangController>();
        ctrl.Init(user.transform, newDir, projectileSpeed, maxDistance, returnSpeed, damage, targetTag);
    }
}
