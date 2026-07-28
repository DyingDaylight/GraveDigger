namespace GraveDigger.Props;

public class DiggerHouse : Decoration
{
    private const int ReputationMultiplier = 10;
    private static readonly string[] HouseSprites =
    {
        "House1", "House2", "House3"
    };

    private int upgradeLevel;
    
    public override bool IsFullyUpgraded => upgradeLevel >= HouseSprites.Length - 1;
    
    public DiggerHouse() : base(HouseSprites[0])
    {
        upgradeLevel = 0;
        IsUnlocked = true;
    }
    
    protected override bool Upgrade()
    {
        if (IsFullyUpgraded)
            return false;
        
        upgradeLevel++;
        ChangeSprite(HouseSprites[upgradeLevel]);
        return true;
    }

    public override int GetReputationValue()
    {
        return upgradeLevel * ReputationMultiplier;
    }
}