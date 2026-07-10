
using System;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.Interactions;

public abstract class Interaction
{
    private readonly IInteractionOwner parent;
    
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
        if (parent == null)
            return new Rectangle();
        
        return parent.InteractionArea;
    }
}