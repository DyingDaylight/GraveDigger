using System;

namespace GraveDigger.Interactions;

public class TombstoneInteraction : Interaction
{
    public TombstoneInteraction(IInteractionOwner collider) : base(collider)
    {
    }
    
    
    public override void Interact()
    {
        Console.WriteLine("Tombstone Interaction");
    }
}