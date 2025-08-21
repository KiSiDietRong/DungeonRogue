using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public SkillState skillSlot1; // Dùng nút lăn chuột
    public SkillState skillSlot2; // Dùng chuột phải

    public float maxCastDistance = 5f; // Khoảng cách tối đa tung chiêu

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (skillSlot1 == null || skillSlot2 == null)
        {
            Debug.LogWarning("Skill slots in SkillManager are not assigned. Please ensure they are initialized.");
        }
    }

    void Update()
    {
        // Nút lăn chuột (Middle Mouse Button)
        if (Input.GetMouseButtonDown(2)) // 2 = Middle mouse button
        {
            if (skillSlot1 != null && skillSlot1.skill != null)
            {
                skillSlot1.Use(gameObject, GetTargetPosition());
            }
        }

        // Chuột phải
        if (Input.GetMouseButtonDown(1)) // 1 = Right mouse button
        {
            if (skillSlot2 != null && skillSlot2.skill != null)
            {
                skillSlot2.Use(gameObject, GetTargetPosition());
            }
        }
    }

    Vector3 GetTargetPosition()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 playerPos = transform.position;
        Vector3 direction = mousePos - playerPos;
        float distance = direction.magnitude;

        if (distance <= maxCastDistance)
        {
            return mousePos;
        }
        else
        {
            return playerPos + direction.normalized * maxCastDistance;
        }
    }
}