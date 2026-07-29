using GraveDigger.Interactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.Props;

public class Decoration : Prop, IInteractionOwner
{
    private const string PlaceholderName = "DecorPlaceholder";
    private readonly string spriteName;
    private readonly Prop placeholder;
    
    public DecorationType DecorationType { get; set; }
    
    public bool IsUnlocked { get; protected set; }
    public virtual bool IsFullyUpgraded => true;
    public bool CanApplyBlueprint => !IsUnlocked || !IsFullyUpgraded;
    
    public HintInteraction HintInteraction { get; private set; }
    public Rectangle InteractionArea {
        get
        {
            if (IsUnlocked)
                return Rectangle.Empty;
            return placeholder.DestRectangle;
        }
    }
    
    public Decoration(string name) : base(name)
    {
        spriteName = name;
        placeholder = new Prop(PlaceholderName);
        placeholder.Transform.Scale = new Vector2(0.3f, 0.3f);
        placeholder.Opacity = 0.3f;
        
        HintInteraction = new HintInteraction(this);
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
        ChangeSprite(spriteName);
        return true;
    }

    protected virtual bool Upgrade()
    {
        return false;
    }

    public override void Update(GameTime gameTime)
    {
        if (!IsUnlocked)
        {
            placeholder.Transform.Position = Transform.Position;
            placeholder.Update(gameTime);
            return;
        }
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsUnlocked)
        {
            placeholder.Draw(spriteBatch);
            return;
        }

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
    
    public void SetHighlighted(bool highlighted)
    {
        // do not highlight
    }
}