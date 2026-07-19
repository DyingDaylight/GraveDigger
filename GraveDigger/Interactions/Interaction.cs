
using System;
using GraveDigger.Data;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.Interactions;

public abstract class Interaction
{
    protected readonly IInteractionOwner interactionOwner;
    
    public string Hint { get; protected set; }
    public bool IsActive { get; set; } = true;

    public Interaction(IInteractionOwner interactionOwner)
    {
        this.interactionOwner = interactionOwner;
    }
    
    public abstract void Interact();
    
    public virtual void OnHoverEnter()
    {
        if (!IsActive)
            return;
        
        interactionOwner.SetHighlighted(true);
    }

    public virtual void OnHoverExit()
    {
        if (!IsActive)
            return;
        
        interactionOwner.SetHighlighted(false);
    }
    
    public Rectangle GetArea()
    {
        return interactionOwner.InteractionArea;
    }
}