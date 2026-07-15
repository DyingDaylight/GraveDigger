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
    
    public event Action OnCloseButton;

    public InventoryWindow()
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
    
    private void HandleCloseClick()
    {
        contextMenu.Hide();
        OnCloseButton?.Invoke();
    }

    private void OpenContextMenu(Point position, InventoryEntry entry)
    {
        ContextMenuAction[] actions =
        {
            new("Use", () => HandleAction(ContextActionType.Use, entry)),
            new("Discard", () => HandleAction(ContextActionType.Discard, entry)),
            new("Sell", () => HandleAction(ContextActionType.Sell, entry)),
            new("Buy", () => HandleAction(ContextActionType.Buy, entry))
        };
        
        contextMenu.Show(new Vector2(position.X, position.Y), actions);
    }
    
    private void HandleAction(ContextActionType action, InventoryEntry entry)
    {
        Console.WriteLine("Action " + action + " on "+ entry);
        //OnContextActionRequested?.Invoke(action, entry);
    }
}