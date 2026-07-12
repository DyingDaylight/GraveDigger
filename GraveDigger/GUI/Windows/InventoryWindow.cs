using System;
using System.Collections.Generic;
using GraveDigger.GUI.Elements;
using GraveDigger.GUI.Layouts;
using GraveDigger.Items;
using GUI.Windows;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Windows;

public class InventoryWindow : Window
{
    private const int Columns = 5;
    private const int Rows = 5;
    
    private readonly HorizontalLayout titleLayout;
    private readonly HorizontalLayout buttonsLayout;
    private readonly GridLayout gridLayout;
    
    private readonly Label nameLabel;
    private readonly Label moneyLabel;
    private readonly Image moneyIcon;
    
    private readonly Button closeButton;
    
    private readonly List<InventorySlot> inventorySlots = new();
    
    public event Action OnCloseButton;

    public InventoryWindow()
    {
        nameLabel = CreateElement<Label>();
        nameLabel.Text = "Inventory";
        
        moneyLabel = CreateElement<Label>();

        moneyIcon = CreateElement<Image>();
        moneyIcon.SetSize(50, 50);
        moneyIcon.SetImage(SpriteManager.GetSprite("Coin").Texture);

        titleLayout = new HorizontalLayout(Bounds);
        titleLayout.HorizontalPadding = 20;
        titleLayout.HorizontalMargins = new Vector2(25, 25);
        titleLayout.SetPosition(Bounds.X, Bounds.Y + 40);
        
        titleLayout.AddElement(nameLabel);
        titleLayout.AddElement(new Spacer());
        titleLayout.AddElement(moneyIcon);
        titleLayout.AddElement(moneyLabel);
        
        closeButton = CreateElement<Button>();
        closeButton.SetText("Close");
        closeButton.OnClick += HandleCloseClick;
        
        buttonsLayout = new HorizontalLayout(Bounds);
        buttonsLayout.HorizontalPadding = 20;
        buttonsLayout.SetPosition(Bounds.X, Bounds.Bottom - 120);
        buttonsLayout.AddElement(closeButton);

        Rectangle gridBounds = new Rectangle(Bounds.X, Bounds.Y + 70,
            Bounds.Width, Bounds.Height - 200);
        gridLayout = new GridLayout(gridBounds);
        gridLayout.SetColumns(Columns);
        gridLayout.SetRows(Rows);
        gridLayout.SetPadding(5, 5);

        CreateInventorySlots();
        
        RefreshLayout();
    }

    public void SetInventory(Inventory inventory)
    {
        if (inventory == null)
            return;
        
        moneyLabel.Text = inventory.Money.ToString();

        ClearSlots();
        
        int index = 0;
        foreach (InventoryEntry entry in inventory.Items.Values)
        {
            if (index >= inventorySlots.Count)
                break;

            inventorySlots[index].SetData(entry);
            index++;
        }
        
        RefreshLayout();
    }
    
    private void CreateInventorySlots()
    {
        int slotCount = Columns * Rows;
        
        for (int i = 0; i < slotCount; i++)
        {
            InventorySlot slots = CreateElement<InventorySlot>();
            
            slots.OnItemRightClicked += HandleContextMenuRequested;

            inventorySlots.Add(slots);
            gridLayout.AddElement(slots);
        }
    }

    private void ClearSlots()
    {
        foreach (InventorySlot slot in inventorySlots)
            slot.SetData(null);
    }
    
    private void RefreshLayout()
    {
        titleLayout.UpdateLayout();
        gridLayout.UpdateLayout();
        buttonsLayout.UpdateLayout();
    }

    private void HandleContextMenuRequested(InventoryEntry entry)
    {
        Console.WriteLine("Context menu for " + entry);
    }
    
    private void HandleCloseClick()
    {
        OnCloseButton?.Invoke();
    }
}