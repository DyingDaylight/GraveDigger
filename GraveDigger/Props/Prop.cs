using GraveDigger.Interactions;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.Props;

public class Prop : Sprite
{
    public enum SortingMode
    {
        ByY,
        Ground
    }

    public SortingMode Mode = SortingMode.ByY;

    public Prop(string name) : base(name)
    {
    }

    public override void Start()
    {
        base.Start();
        
        // Use the sprite's bottom position to determine its draw order.
        // Objects lower on the screen are drawn in front of higher ones.
        UpdateSortingOrder();
    }
    
    protected void UpdateSortingOrder()
    {
        if (Mode == SortingMode.Ground)
        {
            SortingOrder = 1;
            return;
        }
        
        float depth = Bottom / Game1.WorldSize.Y;
        SortingOrder = 1f - MathHelper.Clamp(depth, 0f, 1f);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        UpdateSortingOrder();
    }
}