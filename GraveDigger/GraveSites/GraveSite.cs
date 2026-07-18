using Microsoft.Xna.Framework;
using GraveDigger.Props;
using GraveDigger.Data;

namespace GraveDigger.GraveSites;

public class GraveSite
{
    public Tombstone Tombstone { get; private set; }
    public Prop GraveTile { get; private set; }
    public Prop DirtPile { get; private set; }

    public GraveSite(string tombstoneSpriteName, Vector2 position, GraveSiteData data, GraveSiteState initialState, System.Func<string, Vector2, Prop> createPropObj, System.Func<string, Vector2, Tombstone> createTombstoneObj)
    {
        GraveTile = createPropObj(initialState == GraveSiteState.DugOut ? "grave_digged" : "grave_earth", position);
        GraveTile.CastShadow = false;

        Vector2 tombstonePosition = new Vector2(position.X, position.Y - 200);
        Tombstone = createTombstoneObj(tombstoneSpriteName, tombstonePosition);
    
        Tombstone.GraveTile = GraveTile; 
        Tombstone.GraveTile.Mode = SortingMode.Fixed;
        Tombstone.State = initialState;
        Tombstone.CastShadow = true;
        
        if (initialState == GraveSiteState.DugOut)
        {
            Vector2 dirtPosition = new Vector2(GraveTile.Transform.Position.X, GraveTile.Transform.Position.Y + 80);
            DirtPile = createPropObj("dirt", dirtPosition);
            DirtPile.SortingOrder = GraveTile.SortingOrder - 0.001f; 
        }
    }
    
    public void SyncDirtPile(System.Func<string, Vector2, Prop> createPropObj, System.Action<Prop> removePropObj)
    {
        Vector2 dirtPosition = new Vector2(GraveTile.Transform.Position.X, GraveTile.Transform.Position.Y + 140);

        if (Tombstone.State == GraveSiteState.DugOut && DirtPile == null)
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
}