//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using DG.Tweening;

//public class UpgradeInfoDisplay : MonoBehaviour
//{
//    private static readonly Color[] fiveLevelColors = {
//    new Color(0.2f, 0.6f, 1f),
//    new Color(0.1f, 0.8f, 0.3f),
//    new Color(1f, 0.9f, 0.2f),
//    new Color(1f, 0.5f, 0f),
//    new Color(1f, 0.1f, 0.1f)
//};

//    private static readonly Color[] threeLevelColors = {
//    new Color(0.2f, 0.6f, 1f),
//    new Color(1f, 0.9f, 0.2f),
//    new Color(1f, 0.1f, 0.1f)
//};

//    public Text nameText;
//    public Text levelText;
//    public Text descriptionText;
//    public Image iconImage;

//    public Text upgradeCostText;
//    public Image amberIconImage;
//    public Button upgradeButton;

//    private UpgradeInfo currentUpgrade;
//    private PlayerController player;

//    private Color originalCostColor;
//    [SerializeField] private GameObject upgradeLevelBarParent;
//    [SerializeField] private GameObject segmentPrefab;

//    private int previousDisplayedLevel = -1;
//    private void Start()
//    {
//        player = FindObjectOfType<PlayerController>();
//        upgradeButton.onClick.AddListener(AttemptUpgrade);
//        originalCostColor = upgradeCostText.color;

//        previousDisplayedLevel = -1;
//    }

//    public void ShowUpgradeInfo(UpgradeInfo info) 
//    { 
//        currentUpgrade = info; 
//        nameText.text = info.upgradeName; 
//        levelText.text = "Level: " + info.level; 
//        descriptionText.text = info.GetDynamicDescription(); 
//        if (iconImage != null && info.icon != null) 
//        { 
//            iconImage.sprite = info.icon; 
//            iconImage.enabled = true; 
//        } 
//        if (upgradeCostText != null) 
//        { 
//            upgradeCostText.text = $"{info.currentCost}"; 
//            upgradeCostText.color = originalCostColor; 
//        } 
//        if (amberIconImage != null) 
//        { 
//            amberIconImage.enabled = true; 
//        } 
//        upgradeButton.interactable = info.CanUpgrade(); 
//        if (info.previousDisplayedLevel >= 0 && info.level > info.previousDisplayedLevel) 
//        { 
//            AnimateUpgradeLevelBar(info); 
//        } 
//        else 
//        { 
//            UpdateUpgradeLevelBarInstant(info); 
//        } 
//    }

//    public void AttemptUpgrade()
//    {
//        if (currentUpgrade == null || player == null) return;

//        if (player.Amber >= currentUpgrade.currentCost && currentUpgrade.CanUpgrade())
//        {
//            player.Amber -= currentUpgrade.currentCost; 
//            player.UpdateAmberUI();

//            currentUpgrade.Upgrade();
//            ShowUpgradeInfo(currentUpgrade);
//        }
//        else
//        {
//            TriggerUpgradeFailedFeedback();
//        }
//    }

//    private void TriggerUpgradeFailedFeedback()
//    {
//        if (upgradeCostText != null)
//        {
//            upgradeCostText.color = Color.red;

//            upgradeCostText.transform.DOPunchPosition(Vector3.right * 10f, 0.3f, 10, 1);

//            DOVirtual.DelayedCall(0.5f, () =>
//            {
//                upgradeCostText.color = originalCostColor;
//            });
//        }
//    }

//    private void AnimateUpgradeLevelBar(UpgradeInfo info)
//    {
//        foreach (Transform child in upgradeLevelBarParent.transform)
//            Destroy(child.gameObject);

//        int maxLevel = info.maxLevel;
//        int currentLevel = info.level;
//        Color[] colorSet = maxLevel == 3 ? threeLevelColors : fiveLevelColors;

//        for (int i = 0; i < maxLevel; i++)
//        {
//            GameObject segment = Instantiate(segmentPrefab, upgradeLevelBarParent.transform);
//            Image segmentImage = segment.GetComponent<Image>();

//            if (i < currentLevel - 1)
//            {
//                segmentImage.color = colorSet[i];
//            }
//            else if (i == currentLevel - 1)
//            {
//                segmentImage.color = Color.white;
//                segmentImage.DOColor(colorSet[i], 0.4f)
//                    .SetDelay(0.05f * i)
//                    .SetEase(Ease.OutQuad);
//            }
//            else
//            {
//                   segmentImage.color = Color.white;
//            }
//        }

//        info.previousDisplayedLevel = currentLevel;
//    }

//    private void UpdateUpgradeLevelBarInstant(UpgradeInfo info)
//    {
//        foreach (Transform child in upgradeLevelBarParent.transform)
//            Destroy(child.gameObject);

//        int maxLevel = info.maxLevel;
//        int currentLevel = info.level;

//        Color[] colorSet = maxLevel == 3 ? threeLevelColors : fiveLevelColors;

//        for (int i = 0; i < maxLevel; i++)
//        {
//            GameObject segment = Instantiate(segmentPrefab, upgradeLevelBarParent.transform);
//            Image segmentImage = segment.GetComponent<Image>();

//            segmentImage.color = i < currentLevel ? colorSet[i] : Color.white;
//        }

//        info.previousDisplayedLevel = currentLevel;
//    }


//    private void Update()
//    {
//        if (currentUpgrade != null && Input.GetKeyDown(KeyCode.Space))
//        {
//            AttemptUpgrade();
//        }
//    }
//}
