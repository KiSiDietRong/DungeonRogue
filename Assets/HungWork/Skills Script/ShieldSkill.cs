using UnityEngine;

[CreateAssetMenu(fileName = "ShieldSkill", menuName = "Skills/Shield Skill")]
public class ShieldSkill : Skill
{
    public GameObject shieldPrefab;
    public float duration = 5f;

    public override void Execute(GameObject user, Vector3 target)
    {
        // Nếu đã có khiên thì không tạo thêm
        Transform existingShield = user.transform.Find("ActiveShield");
        if (existingShield != null)
        {
            Destroy(existingShield.gameObject);
        }

        // Tạo khiên bao quanh người chơi
        GameObject shield = Instantiate(shieldPrefab, user.transform.position, Quaternion.identity, user.transform);
        shield.name = "ActiveShield";

        // Gửi thông tin thời gian tồn tại cho khiên
        ShieldController shieldCtrl = shield.GetComponent<ShieldController>();
        if (shieldCtrl != null)
        {
            shieldCtrl.Setup(duration);
        }
    }
}
