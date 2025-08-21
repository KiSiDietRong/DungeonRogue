using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Sand Storm")]
public class SandStormSkill : Skill
{
    [Header("Slow Zone Settings")]
    public GameObject slowZonePrefab;
    public float slowAmount = 0.5f;       
    public float slowDuration = 0.5f;    
    public float zoneRadius = 2f;     
    public float zoneLifetime = 5f;     

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
