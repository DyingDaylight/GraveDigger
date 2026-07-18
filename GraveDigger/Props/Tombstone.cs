using GraveDigger.Data;
using GraveDigger.Interactions;
using Microsoft.Xna.Framework;

namespace GraveDigger.Props;

public class Tombstone : Prop, IInteractionOwner
{
    public GraveSiteData Data { get; private set; }
    public Interaction Interaction { get; set; }
    public Rectangle InteractionArea => DestRectangle;
    
    public GraveDigger.GraveSites.GraveSite ParentSite { get; set; }

    public Tombstone(string name) : base(name) { }

    public void SetData(GraveSiteData data)
    {
        Data = data;
    }

    public void SetHighlighted(bool highlighted)
    {
        Highlighted = highlighted;
    }
    
    public override int GetReputationValue()
    {
        return ParentSite != null ? ParentSite.GetReputationValue() : base.GetReputationValue();
    }
}
