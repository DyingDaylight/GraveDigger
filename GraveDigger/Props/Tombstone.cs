using GraveDigger.Data;
using GraveDigger.Interactions;
using Microsoft.Xna.Framework;

namespace GraveDigger.Props;

public class Tombstone : Prop, IInteractionOwner
{
    public TombstoneData Data { get; private set; }
    
    public Prop GraveTile { get; set; }

    private TombstoneState state;

    public TombstoneState State
    {
        get => state;
        set
        {
            state = value;
            UpdateVisuals();
        }
    }

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
        
        if (State == TombstoneState.Perfect) return false;
    
        State = TombstoneState.Perfect;
    
        return true;
        
        /*if (State != TombstoneState.Broken) return false;

        State = TombstoneState.Perfect;

        return true;
        
        */
    }

    public bool Dig()
    {
        if (State == TombstoneState.DugOut) return false;

        State = TombstoneState.DugOut;

        return true;
    }

    private void UpdateVisuals()
    {
        Transform.Scale = new Vector2(0.3f, 0.3f);

        if (SpriteSheet != null)
        {
            sourceRectangle = SpriteSheet[0, 0];
        }
    }
}
