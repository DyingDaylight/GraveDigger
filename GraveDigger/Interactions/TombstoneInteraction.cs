using System;

namespace GraveDigger.Interactions;

public class TombstoneInteraction : Interaction
{
    public TombstoneInteraction(IInteractionOwner owner) : base(owner)
    {
    }
    
    
    public override void Interact()
    {
        Console.WriteLine("Tombstone Interaction");
    }
}