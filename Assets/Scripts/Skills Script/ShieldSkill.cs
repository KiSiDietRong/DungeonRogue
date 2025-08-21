using UnityEngine;

[CreateAssetMenu(fileName = "ShieldSkill", menuName = "Skills/Shield Skill")]
public class ShieldSkill : Skill
{
    public GameObject shieldPrefab;
    public float duration = 5f;

    public override void Execute(GameObject user, Vector3 target)
    {
        Transform existingShield = user.transform.Find("ActiveShield");
        if (existingShield != null)
        {
            Destroy(existingShield.gameObject);
        }

        GameObject shield = Instantiate(shieldPrefab, user.transform.position, Quaternion.identity, user.transform);
        shield.name = "ActiveShield";

        ShieldController shieldCtrl = shield.GetComponent<ShieldController>();
        if (shieldCtrl != null)
        {
            shieldCtrl.Setup(duration);
        }
    }
}
