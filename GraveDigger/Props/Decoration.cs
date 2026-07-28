using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.Props;

public class Decoration : Prop
{
    public DecorationType DecorationType { get; set; }
    
    public bool IsUnlocked { get; protected set; }
    public virtual bool IsFullyUpgraded => true;
    public bool CanApplyBlueprint => !IsUnlocked || !IsFullyUpgraded;

    public Decoration(string name) : base(name)
    {
    }

    public bool ApplyBlueprint()
    {
        if (!IsUnlocked)
            return Unlock();
        
        if (!IsFullyUpgraded) 
            return Upgrade();
        
        return false;
    }
    
    private bool Unlock()
    {
        if (IsUnlocked)
            return false;
        
        IsUnlocked = true;
        return true;
    }

    protected virtual bool Upgrade()
    {
        return false;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsUnlocked)
            return;
        
        base.Draw(spriteBatch);
    }
    
    public override int GetReputationValue()
    {
        // TODO: depend on decoration type?
        if (IsUnlocked)
        {
            return 10;    
        }
        return 0;
    }
}