using System;
using System.Collections.Generic;
using System.Text;
using GraveDigger.GUI.Windows;

namespace GraveDigger.Items;

public class Inventory
{
    public int Money { get; private set; }
    // TODO: temporary public - Change it!!!!
    public Dictionary<string, InventoryEntry> Items = new();

    public bool Add(ItemData item)
    {
        // TODO: implement Max Stack
        // TODO: think how to store several stacks of same item
        if (Items.ContainsKey(item.Id))
        {
            Items[item.Id].Quantity++;
        }
        else
        {
            InventoryEntry entry = new();
            entry.ItemData = item;
            entry.Quantity = 1;
            Items[entry.ItemData.Id] = entry;
        }
        return true;
    }

    public void AddMoney(int money)
    {
        if (money <= 0)
            return;
        
        Money += money;
    }

    public void SpendMoney(int money)
    {
        if (money <= 0)
            return;
        
        Money -= money;
        Money = Math.Max(Money, 0);
    }
    
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine("Inventory: ");
        foreach (InventoryEntry entry in Items.Values)
        {
            sb.AppendLine(entry.ToString());
        }
        return sb.ToString();
    }
}