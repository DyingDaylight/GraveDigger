using GraveDigger.Data;
using GraveDigger.Interactions;
using Microsoft.Xna.Framework;

namespace GraveDigger.Props;

// TODO: Rename to GraveSite
public class Tombstone : Prop, IInteractionOwner
{
    public GraveSiteData Data { get; private set; }
    public int ReputationValue { get; private set; }
    
    // TODO: private set
    public Prop GraveTile { get; set; }

    private GraveSiteState state;

    public GraveSiteState State
    {
        get => state;
        set
        {
            state = value;
            UpdateVisuals();
        }
    }
    
    public Interaction Interaction { get; set; }

    public Rectangle InteractionArea => destRectangle;

    public Tombstone(string name) : base(name)
    {
        state = GraveSiteState.Intact;
        ReputationValue = 5;
    }

    public void SetData(GraveSiteData data)
    {
        Data = data;
    }

    public void SetHighlighted(bool highlighted)
    {
        Highlighted = highlighted;
    }

    public bool Repair()
    {
        if (State == GraveSiteState.Intact) return false;
    
        State = GraveSiteState.Intact;
    
        return true;
    }

    public bool Dig()
    {
        if (State == GraveSiteState.DugOut) return false;

        State = GraveSiteState.DugOut;

        return true;
    }

    private void UpdateVisuals()
    {
        // TODO: change dirt pile and gave state here
        Transform.Scale = new Vector2(0.3f, 0.3f);

        if (SpriteSheet != null)
        {
            sourceRectangle = SpriteSheet[0, 0];
        }
    }
}
