using UnityEngine;
using UnityEngine.EventSystems;

public class RelicSlotTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int slotIndex;
    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = InventoryManager.Instance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryManager.ShowRelicTooltip(slotIndex, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryManager.HideRelicTooltip();
    }
}
