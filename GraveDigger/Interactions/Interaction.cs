
using System;
using GraveDigger.Data;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.Interactions;

public abstract class Interaction
{
    private readonly IInteractionOwner interactionOwner;
    
    public string Hint { get; protected set; } = string.Empty;
    public Rectangle Area => interactionOwner.InteractionArea;
    
    protected Interaction(IInteractionOwner interactionOwner)
    {
        this.interactionOwner = interactionOwner;
    }
    
    public abstract void Interact();
    
    public virtual void OnHoverEnter()
    {
        interactionOwner.SetHighlighted(true);
    }

    public virtual void OnHoverExit()
    {
        interactionOwner.SetHighlighted(false);
    }
}