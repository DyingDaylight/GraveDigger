using System;

namespace GraveDigger.Interactions;

public class TraderInteraction : Interaction
{
    public event Action OnTradeRequest;
    
    public TraderInteraction(IInteractionOwner parent) : base(parent)
    {
    }

    public override void Interact()
    {
        if (!IsActive)
            return;
        
        OnTradeRequest?.Invoke();
    }
}