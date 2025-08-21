using UnityEngine;
public enum UpgradeType 
{   MaxHPIncrease,
    HealOnNewStage, 
    BonusHPOnBossKill, 
    DealBonusDamageToNonBoss, 
    DealBonusDamageToBoss, 
    GainBonusWhenDefeatBoss, 
    GainCoins, 
    BonusWhenEnteringWorld, 
    BonusWhenEnteringRoom 
}
[System.Serializable]
public class UpgradeInfo
{
    public string upgradeName;
    public int level = 1;
    public int maxLevel = 5;
    [TextArea(2, 5)]
    public string baseDescription;
    public Sprite icon;
    public int currentCost = 50;
    public int costIncreasePerLevel = 50;
    public int valuePerLevel = 10;
    public int previousDisplayedLevel = -1;
    public UpgradeType upgradeType;
    public void Upgrade()
    {
        if (level < maxLevel)
        {
            level++; currentCost += costIncreasePerLevel;
        }
    }

    public bool CanUpgrade()
    {
        return level < maxLevel;
    }
    public string GetDynamicDescription()
    {
        int totalValue = valuePerLevel * level;
        return baseDescription.Replace("{value}", totalValue.ToString());
    }
}