using GraveDigger.Data;
using GraveDigger.Items;
using GraveDigger.Props;

namespace Interfaces;

public interface IGameplayActions
{ 
    void OpenTombstone(Tombstone tombstone);
    void DigGrave(Tombstone tombstone);
    void RepairGrave(Tombstone tombstone);
    void PickupItem(ItemData itemData);
}