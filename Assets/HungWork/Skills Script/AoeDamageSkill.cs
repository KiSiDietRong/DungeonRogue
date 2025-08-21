using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Aoe Damage Skill")]
public class AoeDamageSkill : Skill
{
    [Header("Area Damage Settings")]
    public GameObject areaPrefab;        // Prefab controller (AoeDamageController)
    public GameObject effectPrefab;      // 🌟 Prefab ParticleSystem riêng cho đẹp
    public float radius = 2f;
    public float damagePerSecond = 10f;
    public float duration = 3f;
    public LayerMask targetLayer;
    public string targetTag = "Enemy";

    public override void Execute(GameObject user, Vector3 target)
    {
        if (areaPrefab == null) return;

        // Spawn AOE controller
        GameObject inst = Instantiate(areaPrefab, target, Quaternion.identity);
        AoeDamageController ctrl = inst.GetComponent<AoeDamageController>();
        if (ctrl != null)
        {
            ctrl.Setup(this);
        }

        // 🌟 Spawn hiệu ứng nếu có
        if (effectPrefab != null)
        {
            GameObject fx = Instantiate(effectPrefab, target, Quaternion.identity);
            fx.transform.localScale = Vector3.one * radius * 2f; // scale theo bán kính
            Destroy(fx, duration + 0.2f);
        }
    }
}
