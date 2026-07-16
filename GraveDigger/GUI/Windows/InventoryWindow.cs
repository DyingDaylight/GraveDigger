using System;
using System.Collections.Generic;
using GraveDigger.GUI.Elements;
using GraveDigger.GUI.Layouts;
using GraveDigger.Items;
using GUI.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.GUI.Windows;

public class InventoryWindow : Window
{
    private readonly InventoryView inventoryView;
    private readonly HorizontalLayout buttonsLayout;
    
    private readonly Button closeButton;

    private ItemData selectedItem;
    private Action<ItemData, int> selectedAction;
    
    public event Action<ItemData, int> DiscardRequested;
    public event Action<ItemData, int> UseRequested;
    public event Action OnCloseButton;

    public InventoryWindow(Rectangle parentBounds) : base(parentBounds)
    {
        inventoryView = CreateElement<InventoryView>();
        inventoryView.SetPosition(Bounds.X, Bounds.Y);
        inventoryView.SetSize(Bounds.Width, Bounds.Height - 120);
        
        closeButton = CreateElement<Button>();
        closeButton.SetText("Close");
        closeButton.OnClick += HandleCloseClick;
        
        buttonsLayout = new HorizontalLayout(Bounds);
        buttonsLayout.HorizontalPadding = 20;
        buttonsLayout.SetPosition(Bounds.X, Bounds.Bottom - 120);
        buttonsLayout.AddElement(closeButton);
        
        inventoryView.ContextMenuRequested += OpenContextMenu;
        
        RefreshLayout();
    }

    protected override void RefreshLayout()
    {
        buttonsLayout.UpdateLayout();
    }
    
    public void SetInventory(Inventory inventory)
    {
        inventoryView.SetInventory(inventory);
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
        
        selectedItem = null;
        selectedAction = null;
        
        OnCloseButton?.Invoke();
    }

    private void OpenContextMenu(Point position, InventoryEntry entry)
    {
        List<ContextMenuAction> actions = new();
        
        if (entry.ItemData is FoodItemData)
            actions.Add(new ContextMenuAction("Use", () => Use(entry)));

        actions.Add(new("Discard", () => Discard(entry)));

        contextMenu.Show(new Vector2(position.X, position.Y), actions);
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
        inventoryView.ResetInteraction();

        selectedItem = entry.ItemData;
        selectedAction = action;
        
        quantitySelector.Show(title, 1, entry.Quantity);
    }
}