using UnityEngine;

public class SkillPickup : MonoBehaviour
{
    public Skill skillToGive; // Gán ScriptableObject skill trong Inspector
    public GameObject eIndicatorPrefab;

    private GameObject eIndicatorInstance;
    private bool isPlayerNearby = false;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (eIndicatorPrefab != null)
        {
            eIndicatorInstance = Instantiate(eIndicatorPrefab, transform);
            eIndicatorInstance.transform.localPosition = new Vector3(0, 1f, 0);
            eIndicatorInstance.SetActive(false);
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        isPlayerNearby = distance < 2f;

        if (eIndicatorInstance != null)
            eIndicatorInstance.SetActive(isPlayerNearby);

        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            GiveSkillToPlayer();
        }
    }

    void GiveSkillToPlayer()
    {
        SkillManager manager = player.GetComponent<SkillManager>();
        if (manager != null && skillToGive != null)
        {
            // Ưu tiên slot rỗng
            if (manager.skillSlot1.skill == null)
            {
                manager.skillSlot1.skill = skillToGive;
                manager.skillSlot1.lastUseTime = -999f;
            }
            else if (manager.skillSlot2.skill == null)
            {
                manager.skillSlot2.skill = skillToGive;
                manager.skillSlot2.lastUseTime = -999f;
            }
            else
            {
                Debug.Log("Đã có đủ 2 kỹ năng.");
            }
        }

        Destroy(gameObject); // Nhặt xong thì biến mất
    }
}
