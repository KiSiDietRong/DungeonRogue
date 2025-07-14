using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    public Skill skillSlot1; // Dùng phím Q
    public Skill skillSlot2; // Dùng phím E

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("địt mẹ mày");
            CastSkill(skillSlot1);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("địt mẹ mày");
            CastSkill(skillSlot2);
        }
    }

    void CastSkill(Skill skill)
    {
        if (skill != null && skill.IsReady())
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            skill.Use(gameObject, mousePos);
        }
    }
}