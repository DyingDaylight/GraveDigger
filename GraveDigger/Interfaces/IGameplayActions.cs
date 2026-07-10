using GraveDigger.Data;
using GraveDigger.Props;

namespace Interfaces;

public interface IGameplayActions
{ 
    void OpenTombstone(Tombstone tombstoneData);
    void DigGrave(Tombstone tombstoneData);
    void RepairGrave(Tombstone tombstone);
}