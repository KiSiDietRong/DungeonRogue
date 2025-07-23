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
    SpiritShelter,
    ArchangelsScythe,
    ArcticAxe,
    BitterChisel,
    CryoFreeze,
    ChillingWind,
    DazeClaw,
    FieryImbuement,
    FrostNeedle,
    GiantMace,
    GildedRemains,
    HarvestScythe,
    HunterGrasp,
    ImpactCharm,
    InfernalScale,
    JuicyOpal,
    RecoveryRing,
    RejuvenationGlove,
    TitansWargear,
    TraumaticBlow,
    VoltClaw
}

[System.Serializable]
public enum Rarity
{
    Common,
    Epic,
    Legendary
}

[System.Serializable]
public class Relic
{
    public string relicName;
    public Sprite icon;
    public string effectDescription;
    public RelicType type;
    public Rarity rarityType;
    public string rarity;
    public int cost;
}
