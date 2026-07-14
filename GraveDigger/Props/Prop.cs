using GraveDigger.Core;
using Microsoft.Xna.Framework;

namespace GraveDigger.Props;

public class Prop : Sprite
{
    private const float GroundSortingOrder = 0.99f;
    
    public SortingMode Mode { get; set; } = SortingMode.Dynamic;
    
    public Prop(string name) : base(name)
    {
    }

    public override void Start()
    {
        base.Start();
        UpdateSortingOrder();
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        UpdateSortingOrder();
    }
    
    // Use the sprite's bottom position to determine its draw order.
    // Objects lower on the screen are drawn in front of higher ones.
    private void UpdateSortingOrder()
    {
        if (Mode == SortingMode.Fixed)
        {
            SortingOrder = GroundSortingOrder;
            return;
        }
        
        float depth = Bottom / Game1.WorldSize.Y;
        SortingOrder = 1f - MathHelper.Clamp(depth, 0f, 1f);
    }
}

