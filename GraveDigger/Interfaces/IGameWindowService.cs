using GraveDigger.Data;
using GraveDigger.Items;
using GraveDigger.Props;
using GraveDigger.Systems;
using GraveDigger.GraveSites;

namespace Interfaces;

public interface IGameWindowService
{
    void OpenTombstoneWindow(GraveSite graveSite);
    void OpenInventoryWindow(Inventory inventory);
    void OpenTradeWindow(Inventory inventory, Inventory inventory1);
    
    void CloseCurrentWindow();
    bool IsModalWindowOpen();
    bool IsInventoryOpen();
    void RefreshTombstoneWindow();
}