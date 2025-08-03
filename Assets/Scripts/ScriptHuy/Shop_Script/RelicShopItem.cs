using UnityEngine;
using TMPro;

public class RelicShopItem : MonoBehaviour
{
    public Relic relicData;

    [Header("UI")]
    public GameObject infoCanvas;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI costText;

    private bool playerInRange = false;

    private void Start()
    {
        infoCanvas.SetActive(false);
        nameText.text = relicData.relicName;
        descText.text = relicData.effectDescription;
        costText.text = $"{relicData.cost}G";
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryPurchase();
        }
    }

    private void TryPurchase()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        PlayerController pc = player.GetComponent<PlayerController>();

        if (pc.Gold >= relicData.cost)
        {
            pc.Gold -= relicData.cost;
            pc.UpdateGoldUI();

            InventoryManager.Instance.playerInventory.Add(relicData);
            InventoryManager.Instance.UpdateInventoryUI();
            InventoryManager.Instance.ApplyRelicEffect(relicData);

            Destroy(gameObject); // Xóa relic khỏi shop
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            infoCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            infoCanvas.SetActive(false);
        }
    }
}
