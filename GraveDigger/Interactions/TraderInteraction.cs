using System;

namespace GraveDigger.Interactions;

public class TraderInteraction : Interaction
{
    public TraderInteraction(IInteractionOwner parent) : base(parent)
    {
    }

    public override void Interact()
    {
        Console.WriteLine("Trader Interaction");
    }
}