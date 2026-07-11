using GraveDigger.Data;
using GraveDigger.Props;

namespace Interfaces;

public interface IGameplayActions
{ 
    void OpenTombstone(Tombstone tombstoneData);
    void DigGrave(Tombstone tombstone);
    void RepairGrave(Tombstone tombstone);
}