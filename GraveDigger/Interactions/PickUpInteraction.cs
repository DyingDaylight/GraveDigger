using System;
using GraveDigger.Items;
using GraveDigger.Props;

namespace GraveDigger.Interactions;

public class PickUpInteraction : Interaction
{
    private ItemPickUp owner;
    
    public event Action<ItemPickUp> OnItemPickedUp;
    
    public PickUpInteraction(ItemPickUp parent) : base(parent)
    {
        owner = parent;
        Hint = $"Take {owner.ItemData.Name}";
    }

    public override void Interact()
    {
        OnItemPickedUp?.Invoke(owner);
    }
}