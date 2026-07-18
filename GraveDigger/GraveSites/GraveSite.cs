using GraveDigger.Core;
using Microsoft.Xna.Framework;
using GraveDigger.Props;
using GraveDigger.Data;

namespace GraveDigger.GraveSites;

public class GraveSite
{
    public Transform Transform { get; } = new Transform();
    
    public Tombstone Tombstone { get; private set; }
    public GraveTile GraveTile { get; private set; }
    public Prop DirtPile { get; private set; }

    public GraveSiteState State
    {
        get => GraveTile.State;
        set => GraveTile.State = value;
    }
    
    public bool Dig() 
    {
        if (State == GraveSiteState.DugOut) return false;
        State = GraveSiteState.DugOut;
        DirtPile.Visible = true;
        return true;
    }

    public bool Repair() 
    {
        if (State == GraveSiteState.Intact) return false;
        State = GraveSiteState.Intact;
        DirtPile.Visible = false;
        return true;
    }
    
    public int GetReputationValue()
    {
        return State switch
        {
            GraveSiteState.DugOut => -10,
            GraveSiteState.Broken => -3,
            GraveSiteState.Intact => +2,
            _ => 0
        };
    }

    public void SetTombstone(Tombstone tombstone)
    {
        Tombstone = tombstone;
        Tombstone.Transform.Position = new Vector2(Transform.Position.X, Transform.Position.Y - 200);
        Tombstone.Transform.Scale = new Vector2(0.3f, 0.3f);
        Tombstone.ParentSite = this;
    }

    public void SetGrave(GraveTile graveTile)
    {
        GraveTile = graveTile;
        GraveTile.Transform.Position = new Vector2(Transform.Position.X, Transform.Position.Y);
        GraveTile.Transform.Scale = new Vector2(0.08f, 0.08f);
    }

    public void SetDirt(Prop dirt)
    {
        DirtPile = dirt;
        DirtPile.Transform.Position = new Vector2(Transform.Position.X, Transform.Position.Y + 120);
        DirtPile.Transform.Scale = new Vector2(0.05f, 0.05f);
        DirtPile.Visible = State == GraveSiteState.DugOut;
    }
}