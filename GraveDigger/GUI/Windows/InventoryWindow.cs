using System;
using System.Collections.Generic;
using GraveDigger.GUI.Elements;
using GraveDigger.GUI.Layouts;
using GraveDigger.Items;
using GUI;
using GUI.Windows;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Windows;

public class InventoryWindow : Window
{
    private HorizontalLayout titleLayout;
    private HorizontalLayout buttonsLayout;
    private GridLayout gridLayout;
    
    private Label nameLabel;
    private Label moneyLabel;
    private Image moneyIcon;
    
    private Button closeButton;
    
    private List<InventorySlot> inventorySlots = new();
    
    public event Action OnCloseButton;

    public InventoryWindow()
    {
        nameLabel = CreateElement<Label>();
        nameLabel.Text = "Inventory";
        
        moneyLabel = CreateElement<Label>();

        moneyIcon = CreateElement<Image>();
        moneyIcon.SetSize(50, 50);

        titleLayout = new HorizontalLayout(Bounds);
        titleLayout.Padding = 20;
        titleLayout.PositionY = Bounds.Y + 40;
        titleLayout.horizontalMargins = new Vector2(25, 25);
        titleLayout.AddElement(nameLabel);
        titleLayout.AddElement(new Spacer());
        titleLayout.AddElement(moneyIcon);
        titleLayout.AddElement(moneyLabel);
        titleLayout.UpdateLayout();
        
        closeButton = CreateElement<Button>();
        closeButton.SetText("Close");
        closeButton.OnClick += () => OnCloseButton?.Invoke();
        
        buttonsLayout = new HorizontalLayout(Bounds);
        buttonsLayout.Padding = 20;
        buttonsLayout.PositionY = Bounds.Y + Bounds.Height - 120;
        buttonsLayout.AddElement(closeButton);
        buttonsLayout.UpdateLayout();

        int columns = 5;
        int rows = 5;
        gridLayout = new GridLayout(new Rectangle(Bounds.X, Bounds.Y + 70,
            Bounds.Width, Bounds.Height - 200));
        gridLayout.SetColumns(columns);
        gridLayout.SetRows(rows);
        gridLayout.SetPadding(new Vector2(5, 5));

        for (int i = 0; i < columns * rows; i++)
        {
            InventorySlot inventorySlot = CreateElement<InventorySlot>();
            inventorySlot.OnItemSelected += OnContextMenuRequested;
            gridLayout.AddElement(inventorySlot);
            inventorySlots.Add(inventorySlot);
        }
    }

    private void OnContextMenuRequested(InventoryEntry obj)
    {
        Console.WriteLine("Context menu for " + obj.ToString());
    }

    public void SetInventory(Inventory inventory)
    {
        moneyLabel.Text = inventory.Money.ToString();
        moneyIcon.SetImage(SpriteManager.GetSprite("Coin").Texture);
        int i = 0;
        foreach (InventoryEntry entry in inventory.items.Values)
        {
            if (i < inventorySlots.Count)
            {
                inventorySlots[i].SetData(entry);
            }

            i++;
        }
        Console.WriteLine(inventory.ToString());
        Refresh();
    }
    
    private void Refresh()
    {
        titleLayout.UpdateLayout();
        gridLayout.UpdateLayout();
        buttonsLayout.UpdateLayout();
    }
}