using GraveDigger.Interactions;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.Props;

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
        UpdateSortingOrder();
    }
    
    protected void UpdateSortingOrder()
    {
        float depth = Bottom / Game1.WorldSize.Y;
        SortingOrder = 1f - MathHelper.Clamp(depth, 0f, 1f);
    }
}