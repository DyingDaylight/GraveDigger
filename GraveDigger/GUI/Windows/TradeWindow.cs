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
    
    public event Action OnCloseButton;

    public TradeWindow()
    {
        int width = 1200;
        int height = 800;
        int x = (int) ((1920 - width) * 0.5f);
        int y = (int) ((1080 - height) * 0.5f);
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
    
    private void HandleCloseClick()
    {
        contextMenu.Hide();
        OnCloseButton?.Invoke();
    }
    
    private void OpenPlayerContextMenu(Point position, InventoryEntry entry)
    {
        List<ContextMenuAction> actions = new();
        
        actions.Add(new("Sell", () => HandleAction(ContextActionType.Sell, entry)));
        
        if (entry.ItemData is FoodItemData)
            actions.Add(new ContextMenuAction("Use", () => HandleAction(ContextActionType.Use, entry)));

        actions.Add(new("Discard", () => HandleAction(ContextActionType.Discard, entry)));
        
        contextMenu.Show(new Vector2(position.X, position.Y), actions);
    }
    
    private void OpenMerchantContextMenu(Point position, InventoryEntry entry)
    {
        List<ContextMenuAction> actions = new();
        
        actions.Add(new("Buy", () => HandleAction(ContextActionType.Buy, entry)));
        
        contextMenu.Show(new Vector2(position.X, position.Y), actions);
    }
    
    private void HandleAction(ContextActionType action, InventoryEntry entry)
    {
        Console.WriteLine("Action " + action + " on "+ entry);
        //OnContextActionRequested?.Invoke(action, entry);
    }
}