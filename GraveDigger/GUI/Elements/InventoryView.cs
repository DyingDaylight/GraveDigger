using System;
using System.Collections.Generic;
using GraveDigger.GUI.Layouts;
using GraveDigger.Items;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Elements;

public class InventoryView : UIContainer
{
    private const int Columns = 5;
    private const int Rows = 5;
    
    private readonly Label nameLabel;
    private readonly Label moneyLabel;
    private readonly Image moneyIcon;
    
    private readonly HorizontalLayout titleLayout;
    private readonly GridLayout gridLayout;
    
    private readonly List<InventorySlot> inventorySlots = new();

    public InventoryView()
    {
        nameLabel = CreateElement<Label>();
        nameLabel.Text = "Inventory";
        
        moneyLabel = CreateElement<Label>();

        moneyIcon = CreateElement<Image>();
        moneyIcon.SetSize(50, 50);
        moneyIcon.SetImage(SpriteManager.GetSprite("Coin").Texture);

        titleLayout = new HorizontalLayout(Rectangle.Empty);
        titleLayout.HorizontalPadding = 20;
        titleLayout.HorizontalMargins = new Vector2(25, 25);
        
        titleLayout.AddElement(nameLabel);
        titleLayout.AddElement(new Spacer());
        titleLayout.AddElement(moneyIcon);
        titleLayout.AddElement(moneyLabel);
        
        gridLayout = new GridLayout(Rectangle.Empty);
        gridLayout.SetColumns(Columns);
        gridLayout.SetRows(Rows);
        gridLayout.SetPadding(5, 5);

        CreateInventorySlots();
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
    
    protected override void RefreshLayout()
    {
        int titleWidth = Bounds.Width;
        int titileHeight = 120;

        Rectangle titleBounds = new Rectangle(Bounds.X, Bounds.Y,
            titleWidth, titileHeight);
        titleLayout.SetBounds(titleBounds);
        titleLayout.SetPosition(Bounds.X, Bounds.Y + 40);
        titleLayout.UpdateLayout();
        
        Rectangle gridBounds = new Rectangle(Bounds.X, Bounds.Y + titileHeight,
            Bounds.Width, Bounds.Height - 200);
        gridLayout.SetBounds(gridBounds);
        gridLayout.UpdateLayout();
    }
    
    private void HandleContextMenuRequested(InventoryEntry entry)
    {
        Console.WriteLine("Context menu for " + entry);
    }
}