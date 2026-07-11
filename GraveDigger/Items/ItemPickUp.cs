using GraveDigger.Interactions;
using GraveDigger.Props;
using Microsoft.Xna.Framework;

namespace GraveDigger.Items;

public class ItemPickUp : Prop, IInteractionOwner
{
    public ItemData ItemData { get; private set; }

    public Rectangle InteractionArea => destRectangle;
    public Interaction Interaction { get; set; }
    
    public ItemPickUp(string name) : base(name)
    {
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        UpdateSortingOrder();
    }

    public void SetHighlighted(bool highlighted)
    {
        Highlighted = highlighted;
    }

    public void SetData(ItemData item)
    {
        ItemData = item;
    }
}