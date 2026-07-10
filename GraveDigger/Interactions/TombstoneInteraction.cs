using System;
using GraveDigger.Data;

namespace GraveDigger.Interactions;

public class TombstoneInteraction : Interaction
{
    private TombstoneData tombstoneData;
    public event Action<TombstoneData> OnTombstoneRead;

    public TombstoneInteraction(IInteractionOwner owner) : base(owner)
    {
        Hint = "Read Tombstone";
    }
    
    public override void Interact()
    {
        OnHoverExit();
        OnTombstoneRead?.Invoke(tombstoneData);
    }
    
    public void SetData(TombstoneData tombstoneData)
    {
       this.tombstoneData = tombstoneData;
    }
    
    
}