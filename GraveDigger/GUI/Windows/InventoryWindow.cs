using System;
using System.Collections.Generic;
using GraveDigger.Core;
using GraveDigger.GUI.Elements;
using GraveDigger.GUI.Layouts;
using GraveDigger.Items;
using GUI.Windows;
using Microsoft.Xna.Framework;

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
    
        var closeButton = CreateButton("Close", 200, 60, HandleCloseClick);
    
        buttonsLayout = new HorizontalLayout(Bounds);
        buttonsLayout.HorizontalPadding = 20;
        buttonsLayout.SetPosition(Bounds.X, Bounds.Bottom - 120);
        buttonsLayout.AddElement(closeButton);
    
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
        OnCloseButton?.Invoke();
    }
}