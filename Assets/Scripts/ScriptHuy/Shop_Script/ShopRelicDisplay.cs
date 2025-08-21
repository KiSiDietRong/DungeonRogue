using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopRelicDisplay : MonoBehaviour
{
    public Relic relicData;
    public CanvasGroup infoCanvas;
    public TextMeshProUGUI nameText, effectText, rarityText, costText;
    public Image iconImage;

    private bool playerInRange = false;

    void Start()
    {
        // Gán dữ liệu UI
        if (relicData != null)
        {
            nameText.text = relicData.relicName;
            effectText.text = relicData.effectDescription;
            rarityText.text = relicData.rarity;
            costText.text = relicData.cost.ToString();
            iconImage.sprite = relicData.icon;

            switch (relicData.rarityType)
            {
                case Rarity.Common: rarityText.color = Color.white; break;
                case Rarity.Epic: rarityText.color = new Color(0.6f, 0.2f, 1f); break;
                case Rarity.Legendary: rarityText.color = Color.yellow; break;
            }
        }

        infoCanvas.alpha = 0f;
        infoCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            BuyRelic();
        }
    }

    void BuyRelic()
    {
        InventoryManager inv = InventoryManager.Instance;
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (inv != null && player != null && relicData != null)
        {
            if (player.Gold >= relicData.cost)
            {
                player.Gold -= relicData.cost;
                player.UpdateGoldUI();

                inv.playerInventory.Add(relicData);
                inv.UpdateInventoryUI();
                inv.ApplyRelicEffect(relicData);
                inv.UpdateArmorTextVisibility();

                inv.AddUsedRelic(relicData); // Mark relic as used
                Destroy(gameObject); // Xoá khỏi shop
            }
            else
            {
                Debug.Log("Not enough gold to buy this relic.");
            }
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            infoCanvas.gameObject.SetActive(true);
            infoCanvas.alpha = 1f;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            infoCanvas.alpha = 0f;
            infoCanvas.gameObject.SetActive(false);
        }
    }
}
