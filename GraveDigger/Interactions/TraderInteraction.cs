using System;

namespace GraveDigger.Interactions;

public class TraderInteraction : Interaction
{
    public event Action OnTradeRequested;
    
    public TraderInteraction(IInteractionOwner interactionOwner) : base(interactionOwner)
    {
        Hint = "Trade";
    }

    public override void Interact()
    {
        if (!IsActive)
            return;
        
        OnTradeRequested?.Invoke();
    }
}