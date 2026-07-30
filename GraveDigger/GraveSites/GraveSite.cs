using GraveDigger.Core;
using GraveDigger.Data;
using Microsoft.Xna.Framework;
using GraveDigger.Props;
using Interfaces;

namespace GraveDigger.GraveSites;

public class GraveSite
{
    private const int BaseRepairCost = 10;
    private const int AdditionalDugOutRepairCost = 5;
    
    public Transform Transform { get; } = new Transform();
    
    public Tombstone Tombstone { get; private set; }
    public Prop GraveTile { get; private set; }
    public Prop DirtPile { get; private set; }
    
    public GraveSiteStatus Status { get; private set; }

    public int RepairCost => State switch
    {
        GraveState.DugOut => BaseRepairCost + AdditionalDugOutRepairCost,
        GraveState.Broken => BaseRepairCost,
        _ => 0
    };

    public GraveState State
    {
        get;
        private set
        {
            if (field == value)
                return;

            field = value;
            UpdateVisuals();
        }
    } = GraveState.Intact;

    public GraveSite(GraveSiteStatus status, GraveState state = GraveState.Intact)
    {
        Status = status;
        State = state;
    }
    
    public bool Prepare()
    {
        if (Status != GraveSiteStatus.Locked)
            return false;

        Status = GraveSiteStatus.Prepared;
        UpdateVisuals();
        
        return true;
    }
    
    public bool Occupy(GraveSiteData data, string tombstoneName)
    {
        if (Status != GraveSiteStatus.Prepared)
            return false;

        Status = GraveSiteStatus.Occupied;
        State = GraveState.Intact;

        Tombstone.ChangeSprite(tombstoneName);
        Tombstone.SetData(data);

        UpdateVisuals();
        return true;
    }
    
    public bool Dig() 
    {
        if (Status != GraveSiteStatus.Occupied)
            return false;
        
        if (State == GraveState.DugOut) 
            return false;
        
        State = GraveState.DugOut;
        return true;
    }

    public bool Repair() 
    {
        if (Status != GraveSiteStatus.Occupied)
            return false;
        
        if (State == GraveState.Intact) 
            return false;
        
        State = GraveState.Intact;
        return true;
    }
    
    public bool DecreaseCondition()
    {
        if (Status != GraveSiteStatus.Occupied)
            return false;

        if (State != GraveState.Intact)
            return false;

        State = GraveState.Broken;
        return true;
    }
    
    public int GetReputationValue()
    {
        if (Status == GraveSiteStatus.Locked)
            return 0;
        
        if (Status == GraveSiteStatus.Prepared)
            return -5;
        
        return State switch
        {
            GraveState.DugOut => -10,
            GraveState.Broken => -3,
            GraveState.Intact => +2,
            _ => 0
        };
    }

    public void SetTombstone(Tombstone tombstone)
    {
        Tombstone = tombstone;
        Tombstone?.Transform.Position = new Vector2(Transform.Position.X, Transform.Position.Y - 200);
        Tombstone?.Transform.Scale = new Vector2(0.3f, 0.3f);
        Tombstone?.ParentSite = this;
        UpdateVisuals();
    }

    public void SetGrave(Prop graveTile)
    {
        GraveTile = graveTile;
        GraveTile?.Transform.Position = new Vector2(Transform.Position.X, Transform.Position.Y);
        GraveTile?.Transform.Scale = new Vector2(0.08f, 0.08f);
        GraveTile?.Mode = SortingMode.Fixed;
        UpdateVisuals();
    }

    public void SetDirt(Prop dirt)
    {
        DirtPile = dirt;
        DirtPile?.Transform.Position = new Vector2(Transform.Position.X, Transform.Position.Y + 120);
        DirtPile?.Transform.Scale = new Vector2(0.05f, 0.05f);
        DirtPile?.Visible = State == GraveState.DugOut;
        UpdateVisuals();
    }
    
    private void UpdateVisuals()
    {
        switch (Status)
        {
            case GraveSiteStatus.Locked:
                GraveTile?.ChangeSprite("grave_locked");    
                GraveTile?.Transform.Scale = new Vector2(0.35f, 0.35f);
                break;

            case GraveSiteStatus.Prepared:
                GraveTile?.ChangeSprite("grave_prepared");
                GraveTile?.Transform.Scale = new Vector2(0.18f, 0.18f);
                break;

            case GraveSiteStatus.Occupied:
                GraveTile?.ChangeSprite(State switch
                {
                    GraveState.Intact => "grave_earth",
                    GraveState.Broken => "grave_broken",
                    GraveState.DugOut => "grave_digged"
                });
                GraveTile?.Transform.Scale = new Vector2(0.08f, 0.08f);
                break;
        }
        
        GraveTile?.CastShadow = false;
        
        if (DirtPile != null)
        {
            DirtPile.Visible =
                Status == GraveSiteStatus.Prepared ||
                Status == GraveSiteStatus.Occupied &&
                State == GraveState.DugOut;
        }
        
    }
}