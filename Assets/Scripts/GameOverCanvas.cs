using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverCanvas : MonoBehaviour
{
    private void Update()
    {
        if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = Vector3.zero;
                SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = true;
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
                }
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.ResetPlayer();
                }
            }
            var inv = FindObjectOfType<InventoryManager>();
            if (inv != null) inv.ResetAll();

            var ssm = FindObjectOfType<SkillSelectorManager>();
            if (ssm != null) ssm.ResetAllSkills();

            var sm = FindObjectOfType<SkillManager>();
            if (sm != null) sm.ResetRuntimeFlagsAndSlots();

            var pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.ResetCurrency();

            if (inv != null) inv.ResetAll();
            if (ssm != null) ssm.ResetAllSkills();

            Time.timeScale = 1f;
            gameObject.SetActive(false);
            SceneManager.LoadScene("LobbyScene");
        }
    }
}