using UnityEngine;

[System.Serializable]
public class SkillBut
{
    public string name;
    public int level;
    public int maxLevel = 3;
    public int currentCost;
    public int costPerLevel = 50;
    public string baseDescription;
    public Sprite icon;

    public int lineIndex;         
    public int nodeIndex;       

    public bool IsUnlocked;
    public bool IsAvailable;

    public int RequiredUpgradesToUnlock = 0;

    public string GetDescription()
    {
        if (!IsUnlocked)
        {
            if (nodeIndex == 0 && lineIndex > 0)
            {
                return $"Need to upgrade {RequiredUpgradesToUnlock} times to unlock.";
            }
            else
            {
                return "Need to upgrade before to unlock this.";
            }
        }
        int totalValue = level * 10;
        return baseDescription.Replace("{value}", totalValue.ToString());
    }

    public bool CanUpgrade()
    {
        return IsUnlocked && level < maxLevel;
    }

    public void Upgrade()
    {
        if (CanUpgrade())
        {
            level++;
            currentCost += costPerLevel;
        }
    }
}
