using UnityEngine;

[CreateAssetMenu(fileName = "FireballOrbitSkill", menuName = "Skills/FireTornado")]
public class FireballOrbitSkill : Skill
{
    public GameObject fireballPrefab;
    public int fireballCount = 3;
    public float orbitRadius = 2f;
    public float orbitSpeed = 180f; // độ/giây
    public float duration = 5f;
    public float damage = 10f;

    public override void Execute(GameObject user, Vector3 target)
    {
        // Tạo các fireball xung quanh người chơi
        for (int i = 0; i < fireballCount; i++)
        {
            float angle = (360f / fireballCount) * i;
            GameObject fireball = Instantiate(fireballPrefab, user.transform.position, Quaternion.identity);

            FireTornadoController orbit = fireball.GetComponent<FireTornadoController>();
            orbit.Setup(user.transform, angle, orbitRadius, orbitSpeed, damage, duration);
        }
    }
}
