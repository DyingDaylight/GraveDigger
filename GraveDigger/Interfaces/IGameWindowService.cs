using System;
using GraveDigger.Items;
using GraveDigger.GraveSites;

namespace Interfaces;

public interface IGameWindowService
{
    void OpenTombstoneWindow(GraveSite graveSite, bool hasEnoughMoney);
    void OpenInventoryWindow(Inventory inventory);
    void OpenTradeWindow(Inventory playerInventory, Inventory merchantInventory);
    
    void CloseCurrentWindow();
    bool IsModalWindowOpen();
    bool IsInventoryOpen();
    void RefreshTombstoneWindow(bool hasEnoughMoney);

    event Action MarketClosed;
}