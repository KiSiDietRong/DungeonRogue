//using UnityEngine;

//public class StatueInteract : MonoBehaviour
//{
//    private bool playerInRange = false;
//    private PlayerController playerController;

//    [SerializeField] private GameObject interactPromptUI;   
//    [SerializeField] private GameObject skillTreePanelUI;  

//    void Update()
//    {
//        if (playerInRange && Input.GetKeyDown(KeyCode.F))
//        {
//            ToggleSkillTree();
//        }
//    }

//    private void ToggleSkillTree()
//    {
//        if (skillTreePanelUI != null)
//        {
//            bool isActive = skillTreePanelUI.activeSelf;
//            skillTreePanelUI.SetActive(!isActive);

//            if (playerController != null)
//            {
//                playerController.isSkillTreeOpen = !isActive; 
//            }
//        }
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.CompareTag("Player"))
//        {
//            playerInRange = true;
//            playerController = collision.GetComponent<PlayerController>();

//            if (interactPromptUI != null)
//                interactPromptUI.SetActive(true);
//        }
//    }

//    private void OnTriggerExit2D(Collider2D collision)
//    {
//        if (collision.CompareTag("Player"))
//        {
//            playerInRange = false;
//            if (interactPromptUI != null)
//                interactPromptUI.SetActive(false);
//        }
//    }
//}
