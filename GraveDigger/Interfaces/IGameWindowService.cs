using GraveDigger.Data;
using GraveDigger.Items;
using GraveDigger.Props;

namespace Interfaces;

public interface IGameWindowService
{
    void OpenTombstoneWindow(Tombstone tombstoneData);
    void OpenInventoryWindow(Inventory inventory);
    void CloseCurrentWindow();
    bool IsModalWindowOpen();
    void UpdateTombstoneWindow();
}