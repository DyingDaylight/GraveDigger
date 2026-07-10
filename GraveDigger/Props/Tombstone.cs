using GraveDigger.Data;
using GraveDigger.Interactions;
using Microsoft.Xna.Framework;

namespace GraveDigger.Props;

public class Tombstone : Prop, IInteractionOwner
{
    public TombstoneData Data { get; private set; }
    public TombstoneState State { get; set; }
    public int Value { get; set; }
    public Interaction Interaction { get; set; }
    
    public Rectangle InteractionArea => destRectangle;
    
    public Tombstone(string name) : base(name)
    {
        State = TombstoneState.Perfect;
        Value = 5;
    }

    public void SetData(TombstoneData data)
    {
        Data = data;
    }
    
    public void SetHighlighted(bool highlighted)
    {
        Highlighted = highlighted;
    }

    public bool Repair()
    {
        if (State != TombstoneState.Broken) return false;
        
        State = TombstoneState.Perfect;
        UpdateVisuals();
        
        return true;
    }

    private void UpdateVisuals()
    {
        switch (State)
        {
            
        }
        // TODO: change sprite
    }
    
}