using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public SkillState skillSlot1; 
    public SkillState skillSlot2; 

    public float maxCastDistance = 5f; 

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
        if (Input.GetMouseButtonDown(2)) 
        {
            if (skillSlot1 != null && skillSlot1.skill != null)
            {
                skillSlot1.Use(gameObject, GetTargetPosition());
            }
        }

        if (Input.GetMouseButtonDown(1)) 
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