using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Fireball Skill")]
public class FireballSkill : Skill
{
    public GameObject fireballPrefab;

    public override void Execute(GameObject user, Vector3 target)
    {
        Transform firePoint = user.transform; 

        Vector2 direction = (target - firePoint.position).normalized;

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        fireball.GetComponent<Fireball>().SetDirection(direction);
    }
}