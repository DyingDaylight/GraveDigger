using GraveDigger.Interactions;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger;

public class Prop : Sprite, IHasCollider, IInteractionOwner
{
    public Interaction Interaction { get; set; }

    public Collider Collider { get; set; }
    
    public Rectangle InteractionArea => destRectangle;
    
    public Prop(string name) : base(name)
    {
        Collider = new Collider();
        Collider.Parent = this;
    }

    public override void Start()
    {
        base.Start();
        
        // Use the sprite's bottom position to determine its draw order.
        // Objects lower on the screen are drawn in front of higher ones.
        float depth = Bottom / Game1.ScreenSize.Y;
        SortingOrder = 1f - MathHelper.Clamp(depth, 0f, 1f);
    }
    
    public void SetHighlighted(bool highlighted)
    {
        Highlighted = highlighted;
    }
}