using Microsoft.Xna.Framework;
using GraveDigger.Props;
using GraveDigger.Data;

namespace GraveDigger.GraveSites;

public class GraveSite
{
    public Tombstone Tombstone { get; private set; }
    public Prop GraveTile { get; private set; }
    public Prop DirtPile { get; private set; }
    
    private GraveSiteState state;
    public GraveSiteState State
    {
        get => state;
        set {
            state = value;
            UpdateVisuals();
        }
    }

    public GraveSite(string tombstoneSpriteName, Vector2 position, GraveSiteData data, GraveSiteState initialState, System.Func<string, Vector2, Prop> createPropObj, System.Func<string, Vector2, Tombstone> createTombstoneObj)
    {
        GraveTile = createPropObj(initialState == GraveSiteState.DugOut ? "grave_digged" : "grave_earth", position);
        GraveTile.CastSHadow = false;
        
        GraveTile.Mode = SortingMode.Fixed;

        Vector2 tombstonePosition = new Vector2(position.X, position.Y - 200);
        Tombstone = createTombstoneObj(tombstoneSpriteName, tombstonePosition);
    
        State = initialState;
        
        if (initialState == GraveSiteState.DugOut)
        {
            Vector2 dirtPosition = new Vector2(GraveTile.Transform.Position.X, GraveTile.Transform.Position.Y + 80);
            DirtPile = createPropObj("dirt", dirtPosition);
            DirtPile.SortingOrder = GraveTile.SortingOrder - 0.001f; 
        }
    }
    
    private void UpdateVisuals()
    {
        GraveTile.ChangeSprite(State switch {
            GraveSiteState.DugOut => "grave_digged",
            GraveSiteState.Broken => "grave_broken",
            _ => "grave_earth"
        });
    }
    
    public void SyncDirtPile(System.Func<string, Vector2, Prop> createPropObj, System.Action<Prop> removePropObj)
    {
        Vector2 dirtPosition = new Vector2(GraveTile.Transform.Position.X, GraveTile.Transform.Position.Y + 140);

        if (State == GraveSiteState.DugOut && DirtPile == null)
        {
            DirtPile = createPropObj("dirt", dirtPosition);
            DirtPile.Transform.Scale = new Vector2(0.05f, 0.05f);
        }
    }
    
    public void RemoveDirtPile(System.Action<Prop> removePropObj)
    {
        if (DirtPile != null)
        {
            removePropObj(DirtPile);
            DirtPile = null;
        }
    }
    
    public bool Dig() 
    {
        if (State == GraveSiteState.DugOut) return false;
        State = GraveSiteState.DugOut;
        return true;
    }

    public bool Repair() 
    {
        if (State == GraveSiteState.Intact) return false;
        State = GraveSiteState.Intact;
        return true;
    }
    
    public int GetReputationValue()
    {
        return State switch
        {
            GraveSiteState.DugOut => -10,
            GraveSiteState.Broken => -3,
            GraveSiteState.Intact => +2
        };
    }
}