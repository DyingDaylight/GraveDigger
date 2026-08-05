namespace GraveDigger.Props;

public class UpgradableDecoration : Decoration
{
    private const int ReputationMultiplier = 10;

    private int upgradeLevel;
    private int maxUpgrades = 3;
    private string spriteName;
    
    public override bool IsFullyUpgraded => upgradeLevel >= maxUpgrades - 1;
    
    public UpgradableDecoration(string name) : base($"{name}1")
    {
        spriteName = name;
        upgradeLevel = 0;
        IsUnlocked = true;
        Collider.IsActive = true;
    }
    
    protected override bool Upgrade()
    {
        if (IsFullyUpgraded)
            return false;
        
        upgradeLevel++;
        ChangeSprite($"{spriteName}{upgradeLevel + 1}");
        return true;
    }

    public override int GetReputationValue()
    {
        return upgradeLevel * ReputationMultiplier;
    }
}