using UnityEngine;

[CreateAssetMenu(menuName = "Skills/SunFire Skill")]
public class SunFireSkill : Skill
{
    public GameObject SunFirePrefab;

    public override void Execute(GameObject user, Vector3 target)
    {
        Transform firePoint = user.transform; // Hoặc 1 transform firePoint riêng nếu có

        Vector2 direction = (target - firePoint.position).normalized;

        GameObject SunFire = Instantiate(SunFirePrefab, firePoint.position, Quaternion.identity);
        SunFire.GetComponent<SunFire>().SetDirection(direction);
    }
}