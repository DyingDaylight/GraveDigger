using GraveDigger.Data;
using GraveDigger.Interactions;
using Microsoft.Xna.Framework;

namespace GraveDigger.Props;

public class Tombstone : Prop, IInteractionOwner
{
    public TombstoneData Data { get; private set; }
    public Interaction Interaction { get; set; }
    
    public Rectangle InteractionArea => destRectangle;
    
    public Tombstone(string name) : base(name)
    {
    }

    public void SetData(TombstoneData data)
    {
        Data = data;
    }
    
    public void SetHighlighted(bool highlighted)
    {
        Highlighted = highlighted;
    }
    
}