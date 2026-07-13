using System;
using GraveDigger.Data;
using GraveDigger.Props;

namespace GraveDigger.Interactions;

public class TombstoneInteraction : Interaction
{
    private Tombstone owner;
    public event Action<Tombstone> OnTombstoneRead;

    public TombstoneInteraction(Tombstone owner) : base(owner)
    {
        Hint = "Read Tombstone";
        this.owner = owner;
    }
    
    public override void Interact()
    {
        OnTombstoneRead?.Invoke(owner);
    }
}