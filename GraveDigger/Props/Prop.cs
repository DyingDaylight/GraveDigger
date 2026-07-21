using GraveDigger.Core;
using GraveDigger.Utils;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.Props;

public class Prop : Sprite, IReputationContributor
{
    private const float GroundSortingOrder = 0.99f;
    
    public SortingMode Mode { get; set; } = SortingMode.Dynamic;

    
    public Prop(string name) : base(name) { }

    public override void Start()
    {
        base.Start();
        UpdateSortingOrder();
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (Mode == SortingMode.Dynamic)
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
        
        SortingOrder = SortingUtility.CalculateByY(Bottom);
    }
}

