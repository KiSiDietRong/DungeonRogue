using UnityEngine;

[System.Serializable]
public enum RelicType
{
    BobsContainmentField,
    BonePlate,
    ConduitSpike,
    DiscountCard,
    DoomShell,
    EmpoweredBangle,
    FuryCharm,
    GlacialLight,
    RazorClaw,
    SpiritShelter
}

[System.Serializable]
public class Relic
{
    public string relicName;
    public Sprite icon;
    public string effectDescription;
    public RelicType type;
}
