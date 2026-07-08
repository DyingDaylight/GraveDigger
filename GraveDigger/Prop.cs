using Microsoft.Xna.Framework;

namespace GraveDigger;

public class Prop : Sprite
{
    public Prop(string name) : base(name)
    {
    }

    public override void Start()
    {
        base.Start();
        
        // Use the sprite's bottom position to determine its draw order.
        // Objects lower on the screen are drawn in front of higher ones.
        float depth = Bottom / Game1.ScreenSize.Y;
        SortingOrder = 1f - MathHelper.Clamp(depth, 0f, 1f);
    }
}