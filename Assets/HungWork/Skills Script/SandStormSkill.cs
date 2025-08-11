using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Sand Storm")]
public class SandStormSkill : Skill
{
    [Header("Slow Zone Settings")]
    public GameObject slowZonePrefab;
    public float slowAmount = 0.5f;       // % giảm tốc (0.5 = giảm 50%)
    public float slowDuration = 0.5f;     // thời gian làm chậm sau khi ra khỏi zone
    public float zoneRadius = 2f;         // bán kính zone
    public float zoneLifetime = 5f;       // tồn tại bao lâu

    public override void Execute(GameObject user, Vector3 target)
    {
        GameObject zone = Instantiate(slowZonePrefab, target, Quaternion.identity);
        SandStormController slowCtrl = zone.GetComponent<SandStormController>();

        if (slowCtrl != null)
        {
            slowCtrl.slowAmount = slowAmount;
            slowCtrl.slowDuration = slowDuration;
            slowCtrl.zoneRadius = zoneRadius;
            slowCtrl.zoneLifetime = zoneLifetime;
        }
    }
}
