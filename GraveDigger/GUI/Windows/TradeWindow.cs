using System;
using System.Collections.Generic;
using GraveDigger.GUI.Elements;
using GraveDigger.GUI.Layouts;
using GraveDigger.Items;
using GUI.Windows;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Windows;

public class TradeWindow : Window
{
    private readonly InventoryView playerInventory;
    private readonly InventoryView merchantInventory;
    private readonly HorizontalLayout inventoriesLayout;
    private readonly HorizontalLayout buttonsLayout;
    
    private readonly Button closeButton;
    
    private ItemData selectedItem;
    private Action<ItemData, int> selectedAction;
    
    public event Action<ItemData, int> DiscardRequested;
    public event Action<ItemData, int> UseRequested;
    public event Action<ItemData, int> SellRequested;
    public event Action<ItemData, int> BuyRequested;
    public event Action OnCloseButton;

    public TradeWindow(Rectangle parentBounds) : base(parentBounds)
    {
        int width = 1200;
        int height = 800;
        int x = parentBounds.X + (parentBounds.Width - width) / 2;
        int y = parentBounds.Y + (parentBounds.Height - height) / 2;
        Bounds = new Rectangle(x, y, width, height);
        
        int halfWidth = (int) (Bounds.Width * 0.5f);
        int buttonsHeight = 120;
        
        playerInventory = CreateElement<InventoryView>();
        playerInventory.SetSize(halfWidth, Bounds.Height - buttonsHeight);
        
        merchantInventory = CreateElement<InventoryView>();
        merchantInventory.SetSize(halfWidth, Bounds.Height - buttonsHeight);

        Rectangle inventoryRect = new Rectangle(Bounds.X, Bounds.Y, 
            Bounds.Width, Bounds.Height - buttonsHeight);
        inventoriesLayout = new HorizontalLayout(inventoryRect);
        
        inventoriesLayout.AddElement(playerInventory);
        inventoriesLayout.AddElement(merchantInventory);
        
        closeButton = CreateElement<Button>();
        closeButton.SetText("Close");
        closeButton.OnClick += HandleCloseClick;
        
        buttonsLayout = new HorizontalLayout(Bounds);
        buttonsLayout.SetPosition(Bounds.X, Bounds.Bottom - buttonsHeight);
        buttonsLayout.AddElement(closeButton);

        playerInventory.ContextMenuRequested += OpenPlayerContextMenu;
        merchantInventory.ContextMenuRequested += OpenMerchantContextMenu;
        
        RefreshLayout();
    }
    
    protected override void RefreshLayout()
    {
        inventoriesLayout.UpdateLayout();
        buttonsLayout.UpdateLayout();
    }
    
    public void SetInventories(Inventory playerInventory, Inventory merchantInventory)
    {
        this.playerInventory.SetTitle("Grave Digger");
        this.playerInventory.SetInventory(playerInventory);
        
        this.merchantInventory.SetTitle("Merchant");
        this.merchantInventory.SetInventory(merchantInventory);
    }
    
    protected override void HandleQuantityConfirmed(int amount)
    {
        if (selectedItem == null || selectedAction == null)
            return;
        
        selectedAction?.Invoke(selectedItem, amount);
        
        selectedItem = null;
        selectedAction = null;
    }
    
    private void HandleCloseClick()
    {
        contextMenu.Hide();
        quantitySelector.Hide();
        OnCloseButton?.Invoke();
    }
    
    private void OpenPlayerContextMenu(Point position, InventoryEntry entry)
    {
        List<ContextMenuAction> actions = new();
        
        actions.Add(new("Sell", () => Sell(entry)));
        
        if (entry.ItemData is FoodItemData)
            actions.Add(new ContextMenuAction("Use", () => Use(entry)));

        actions.Add(new("Discard", () => Discard(entry)));
        
        contextMenu.Show(new Vector2(position.X, position.Y), actions);
    }
    
    private void OpenMerchantContextMenu(Point position, InventoryEntry entry)
    {
        List<ContextMenuAction> actions = new();
        
        actions.Add(new("Buy", () => Buy(entry)));
        
        contextMenu.Show(new Vector2(position.X, position.Y), actions);
    }

    private void Sell(InventoryEntry entry)
    {
        RequestQuantity($"Sell {entry.ItemData.Name}", entry, SellRequested);
    }
    
    private void Buy(InventoryEntry entry)
    {
        RequestQuantity($"Buy {entry.ItemData.Name}", entry, BuyRequested);
    }
    
    private void Use(InventoryEntry entry)
    {
        RequestQuantity($"Use {entry.ItemData.Name}", entry, UseRequested);
    }
    
    private void Discard(InventoryEntry entry)
    {
        RequestQuantity($"Discard {entry.ItemData.Name}", entry, DiscardRequested);
    }
    
    private void RequestQuantity(string title, InventoryEntry entry, Action<ItemData, int> action)
    {
        contextMenu.Hide();
        playerInventory.ResetInteraction();
        merchantInventory.ResetInteraction();
        
        selectedItem = entry.ItemData;
        selectedAction = action;
        
        quantitySelector.Show(title, 1, entry.Quantity);
    }
}