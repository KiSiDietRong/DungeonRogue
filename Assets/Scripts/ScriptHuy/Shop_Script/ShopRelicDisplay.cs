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
    private int discountedCost; // Giá sau khi giảm

    void Start()
    {
        // Gán dữ liệu UI
        if (relicData == null)
        {
            Debug.LogError("relicData is null in ShopRelicDisplay!");
            return;
        }
        if (costText == null)
        {
            Debug.LogError("costText is null in ShopRelicDisplay!");
            return;
        }

        InventoryManager inv = InventoryManager.Instance;
        bool hasDiscountCard = inv != null && inv.playerInventory.Exists(relic => relic.type == RelicType.DiscountCard);

        // Tính giá giảm 25% nếu có DiscountCard
        discountedCost = hasDiscountCard ? Mathf.FloorToInt(relicData.cost * 0.75f) : relicData.cost;

        nameText.text = relicData.relicName;
        effectText.text = relicData.effectDescription;
        rarityText.text = relicData.rarity;
        costText.text = hasDiscountCard ? $"{relicData.cost} → {discountedCost}" : $"{discountedCost}";
        iconImage.sprite = relicData.icon;

        switch (relicData.rarityType)
        {
            case Rarity.Common: rarityText.color = Color.white; break;
            case Rarity.Epic: rarityText.color = new Color(0.6f, 0.2f, 1f); break;
            case Rarity.Legendary: rarityText.color = Color.yellow; break;
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
        PlayerController player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();

        if (inv != null && player != null && relicData != null)
        {
            // Kiểm tra xem có DiscountCard và giá đã giảm không
            bool hasDiscountCard = inv.playerInventory.Exists(relic => relic.type == RelicType.DiscountCard);
            bool usedDiscount = hasDiscountCard && discountedCost < relicData.cost;

            // Sử dụng giá giảm để kiểm tra và trừ vàng
            if (player.Gold >= discountedCost)
            {
                player.Gold -= discountedCost;
                player.UpdateGoldUI();

                inv.playerInventory.Add(relicData);
                inv.UpdateInventoryUI();
                inv.ApplyRelicEffect(relicData);
                inv.UpdateArmorTextVisibility();

                inv.AddUsedRelic(relicData); // Mark relic as used

                // Xóa DiscountCard nếu đã sử dụng giá giảm
                if (usedDiscount)
                {
                    inv.RemoveRelic(RelicType.DiscountCard);
                    Debug.Log("DiscountCard removed after purchase.");
                }

                Destroy(gameObject); // Xoá khỏi shop
                Debug.Log($"Bought relic: {relicData.relicName} for {discountedCost} gold (original: {relicData.cost}).");
            }
            else
            {
                Debug.Log($"Not enough gold to buy {relicData.relicName}. Need {discountedCost}, have {player.Gold}.");
            }
        }
        else
        {
            Debug.LogError("Cannot buy relic: InventoryManager, PlayerController, or relicData is null.");
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

    // Hàm để cập nhật giá hiển thị khi trạng thái DiscountCard thay đổi
    public void UpdateCostDisplay()
    {
        if (relicData == null)
        {
            Debug.LogError("relicData is null in UpdateCostDisplay!");
            return;
        }
        if (costText == null)
        {
            Debug.LogError("costText is null in UpdateCostDisplay!");
            return;
        }

        InventoryManager inv = InventoryManager.Instance;
        bool hasDiscountCard = inv != null && inv.playerInventory.Exists(relic => relic.type == RelicType.DiscountCard);
        discountedCost = hasDiscountCard ? Mathf.FloorToInt(relicData.cost * 0.75f) : relicData.cost;
        costText.text = hasDiscountCard ? $"{relicData.cost} → {discountedCost}" : $"{discountedCost}";
    }
}