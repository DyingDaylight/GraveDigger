using GraveDigger.Interactions;
using GraveDigger.Items;
using Microsoft.Xna.Framework;

namespace GraveDigger.Props;

public class ItemPickUp : Prop, IInteractionOwner
{
    public ItemData ItemData { get; private set; }
    public Interaction Interaction { get; set; }
    public Rectangle InteractionArea => DestRectangle;
    
    public ItemPickUp(string name) : base(name)
    {
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