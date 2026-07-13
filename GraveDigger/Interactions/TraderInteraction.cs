using System;

namespace GraveDigger.Interactions;

public class TraderInteraction : Interaction
{
    public TraderInteraction(IInteractionOwner tombstone) : base(tombstone)
    {
    }

    public override void Interact()
    {
        Console.WriteLine("Trader Interaction");
    }
}