using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Fire Breath")]
public class FireBreathSkill : Skill
{
    [Header("Flame Settings")]
    public GameObject flamePrefab;
    public float duration = 3f;
    public float damagePerSecond = 10f;
    public float tickInterval = 0.5f;
    public float coneAngle = 60f;
    public float range = 3f;
    public string targetTag = "Enemy";

    public override void Execute(GameObject user, Vector3 target)
    {
        Vector3 dir = (target - user.transform.position).normalized;
        GameObject flameGO = Instantiate(flamePrefab, user.transform.position, Quaternion.identity);

        FireBreathController controller = flameGO.GetComponent<FireBreathController>();
        if (controller != null)
        {
            controller.Initialize(user, dir, duration, damagePerSecond, tickInterval, coneAngle, range, targetTag);
        }
    }
}