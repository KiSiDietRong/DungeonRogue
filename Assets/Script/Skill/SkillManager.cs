using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    public SkillState skillSlot1; // Dùng phím Q
    public SkillState skillSlot2; // Dùng phím E

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

            Debug.Log("Casting skill: " + skillState.skill.skillName);
            skillState.Use(gameObject, mousePos);
        }
    }
}