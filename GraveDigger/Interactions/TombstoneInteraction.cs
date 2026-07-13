using System;
using GraveDigger.Data;
using GraveDigger.Props;

namespace GraveDigger.Interactions;

public class TombstoneInteraction : Interaction
{
    private readonly Tombstone tombstone;
    public event Action<Tombstone> OnTombstoneRead;

    public TombstoneInteraction(Tombstone tombstone) : base(tombstone)
    {
        Hint = "Read Tombstone";
        this.tombstone = tombstone;
    }
    
    public override void Interact()
    {
        OnTombstoneRead?.Invoke(tombstone);
    }
}