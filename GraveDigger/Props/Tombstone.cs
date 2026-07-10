using GraveDigger.Interactions;
using Microsoft.Xna.Framework;

namespace GraveDigger.Props;

public class Tombstone : Prop, IInteractionOwner
{
    public Interaction Interaction { get; set; }
    
    public Rectangle InteractionArea => destRectangle;
    
    public Tombstone(string name) : base(name)
    {

    }
    
    public void SetHighlighted(bool highlighted)
    {
        Highlighted = highlighted;
    }
    
}