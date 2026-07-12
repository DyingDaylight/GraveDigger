using GraveDigger.Data;
using GraveDigger.Items;
using GraveDigger.Props;

namespace Interfaces;

public interface IGameWindowService
{
    void OpenTombstoneWindow(Tombstone tombstone);
    void OpenInventoryWindow(Inventory inventory);
    void CloseCurrentWindow();
    bool IsModalWindowOpen();
    bool IsInventoryOpen();
    void RefreshTombstoneWindow();
}