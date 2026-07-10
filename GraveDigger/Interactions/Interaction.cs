
using System;
using GraveDigger.Data;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.Interactions;

public abstract class Interaction
{
    protected readonly IInteractionOwner parent;
    
    public string Hint { get; protected set; }
    
    
    public Interaction(IInteractionOwner parent)
    {
        this.parent = parent;
    }
    
    public abstract void Interact();
    
    public virtual void OnHoverEnter()
    {
        parent.SetHighlighted(true);
    }

    public virtual void OnHoverExit()
    {
        parent.SetHighlighted(false);
    }
    
    public Rectangle GetArea()
    {
        return parent.InteractionArea;
    }
}