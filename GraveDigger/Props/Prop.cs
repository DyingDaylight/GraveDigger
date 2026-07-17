using GraveDigger.Core;
using GraveDigger.Utils;
using Microsoft.Xna.Framework;

namespace GraveDigger.Props;

public class Prop : Sprite
{
    private const float GroundSortingOrder = 0.99f;
    
    private string propName;
    public SortingMode Mode { get; set; } = SortingMode.Dynamic;

    
    public Prop(string name) : base(name)
    {
        propName = name;
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

    public virtual int GetReputationValue()
    {
        return 0;
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
        // to draw earth after player
        float baseOrder = SortingUtility.CalculateByY(Bottom);
    
        if (propName.StartsWith("grave_"))
        {
            SortingOrder = baseOrder + 0.050f; 
        }
        else
        {
            SortingOrder = baseOrder;
        }
        // SortingOrder = SortingUtility.CalculateByY(Bottom);
    }
}

