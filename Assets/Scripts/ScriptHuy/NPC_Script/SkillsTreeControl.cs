using UnityEngine;

public class SkillsTreeControl : MonoBehaviour
{
    private PlayerController playerController;

    private void OnEnable()
    {
        if (playerController == null)
            playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();

        if (playerController != null)
            playerController.isSkillTreeOpen = true;
    }

    private void OnDisable()
    {
        if (playerController == null)
            playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();

        if (playerController != null)
            playerController.isSkillTreeOpen = false;
    }
}
