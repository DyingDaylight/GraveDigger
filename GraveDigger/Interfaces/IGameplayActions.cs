using System;
using GraveDigger.Data;
using GraveDigger.Items;
using GraveDigger.Props;

namespace Interfaces;

public interface IGameplayActions
{ 
    void OpenTombstone(Tombstone tombstoneData);
    void DigGrave(Tombstone tombstone);
    void RepairGrave(Tombstone tombstone);
    void PickupItem(ItemData itemData);
    
    public event Action<Tombstone> OnGraveDug;
    public event Action<Tombstone> OnGraveRepaired;
}