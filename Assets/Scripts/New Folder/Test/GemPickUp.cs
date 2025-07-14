using UnityEngine;

public class GemPickup : MonoBehaviour
{
    public int hpBoost = 10;
    public float speedBoost = 1f;
    public int attackBoost = 1;

    public GameObject interactionText;

    private bool playerInRange = false;
    private PlayerInfo playerStats;

    private void Start()
    {
        interactionText.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (playerStats != null)
            {
                playerStats.IncreaseStats(hpBoost, speedBoost, attackBoost);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactionText.SetActive(true);
            playerInRange = true;
            playerStats = collision.GetComponent<PlayerInfo>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactionText.SetActive(false);
            playerInRange = false;
            playerStats = null;
        }
    }
}
