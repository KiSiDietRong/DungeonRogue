using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeHoverButton : MonoBehaviour, IPointerEnterHandler
{
    public UpgradeInfo upgradeInfo;
    private UpgradeInfoDisplay infoDisplay;
    public static GameObject currentHoveredUp;

    private void Start()
    {
        infoDisplay = FindObjectOfType<UpgradeInfoDisplay>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentHoveredUp = transform.parent.gameObject;
        infoDisplay?.ShowUpgradeInfo(upgradeInfo);
    }
}
