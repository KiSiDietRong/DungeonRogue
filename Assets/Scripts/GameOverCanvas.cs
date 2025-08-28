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
            var ssm = FindObjectOfType<SkillSelectorManager>();
            var sm = FindObjectOfType<SkillManager>();
            var pc = player.GetComponent<PlayerController>();

            if (inv != null) inv.ResetAll();          // reset relic
            if (ssm != null) ssm.ResetAllSkills();    // reset skill
            if (sm != null) sm.ResetRuntimeFlagsAndSlots();
            if (pc != null) pc.ResetCurrency();

            // Đồng bộ UI sau reset
            if (inv != null) inv.UpdateInventoryUI();
            if (ssm != null) ssm.UpdateUI();

            Time.timeScale = 1f;
            gameObject.SetActive(false);
            SceneManager.LoadScene("LobbyScene");
        }
    }
}