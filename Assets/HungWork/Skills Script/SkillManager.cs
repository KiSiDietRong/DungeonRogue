using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public SkillState skillSlot1; // Dùng phím Q
    public SkillState skillSlot2; // Dùng phím E

    public float maxCastDistance = 5f; // Khoảng cách tối đa tung chiêu

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CastSkill(skillSlot1);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            CastSkill(skillSlot2);
        }
    }

    void CastSkill(SkillState skillState)
    {
        if (skillState != null && skillState.IsReady())
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            Vector3 playerPos = transform.position;
            Vector3 direction = mousePos - playerPos;
            float distance = direction.magnitude;

            Vector3 castPos;

            if (distance <= maxCastDistance)
            {
                castPos = mousePos;
            }
            else
            {
                // Giới hạn tung chiêu tại rìa phạm vi
                castPos = playerPos + direction.normalized * maxCastDistance;
            }

            skillState.Use(gameObject, castPos);
        }
    }
}
