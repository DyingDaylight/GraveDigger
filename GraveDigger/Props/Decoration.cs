using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.Props;

public class Decoration : Prop
{
    public DecorationType DecorationType { get; set; }
    
    public bool IsUnlocked { get; private set; }
    
    public Decoration(string name) : base(name)
    {
        IsUnlocked = false;
    }
    
    public void Unlock()
    {
        if (IsUnlocked)
            return;
        
        IsUnlocked = true;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsUnlocked)
            return;
        
        base.Draw(spriteBatch);
    }
}