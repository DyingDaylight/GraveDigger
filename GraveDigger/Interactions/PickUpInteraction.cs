using System;
using GraveDigger.Items;
using GraveDigger.Props;

namespace GraveDigger.Interactions;

public class PickUpInteraction : Interaction
{
    private readonly ItemPickUp pickUpItem;
    
    public event Action<ItemPickUp> OnItemPickedUp;
    
    public PickUpInteraction(ItemPickUp pickUpItem) : base(pickUpItem)
    {
        this.pickUpItem = pickUpItem;
        Hint = $"Take {this.pickUpItem.ItemData.Name}";
    }

    public override void Interact()
    {
        OnItemPickedUp?.Invoke(pickUpItem);
    }
}