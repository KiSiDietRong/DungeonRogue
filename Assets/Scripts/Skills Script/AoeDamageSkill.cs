using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Aoe Damage Skill")]
public class AoeDamageSkill : Skill
{
    [Header("Area Damage Settings")]
    public GameObject areaPrefab;        
    public GameObject effectPrefab;      
    public float radius = 2f;
    public float damagePerSecond = 10f;
    public float duration = 3f;
    public LayerMask targetLayer;
    public string targetTag = "Enemy";

    public override void Execute(GameObject user, Vector3 target)
    {
        if (areaPrefab == null) return;

        GameObject inst = Instantiate(areaPrefab, target, Quaternion.identity);
        AoeDamageController ctrl = inst.GetComponent<AoeDamageController>();
        if (ctrl != null)
        {
            ctrl.Setup(this);
        }

        if (effectPrefab != null)
        {
            GameObject fx = Instantiate(effectPrefab, target, Quaternion.identity);
            fx.transform.localScale = Vector3.one * radius * 2f; 
            Destroy(fx, duration + 0.2f);
        }
    }
}
