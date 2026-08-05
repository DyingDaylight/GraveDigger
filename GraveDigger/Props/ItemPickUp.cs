using System;
using GraveDigger.Core;
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
        Collider.Triggered += OnTriggerEnter;
    }

    public void SetHighlighted(bool highlighted)
    {
        Highlighted = highlighted;
    }

    public void SetData(ItemData item)
    {
        ItemData = item;
    }
    
    public void OnTriggerEnter(Collider self, Collider other)
    {
        if (other.Layer != CollisionLayer.Player)
            return;
        
        // TODO:
        // Trigger pickup currently reuses the existing interaction pipeline.
        // A cleaner implementation would queue pickup requests until collision
        // processing is complete to avoid modifying the collider collection
        // during iteration. Left as-is to keep the implementation simple.
        Interaction.Interact();
    }
}