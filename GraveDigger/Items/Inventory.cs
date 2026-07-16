using System;
using System.Collections.Generic;
using System.Text;

namespace GraveDigger.Items;

public class Inventory
{
    private const int Capacity = 25;
    public int Money { get; private set; }
    
    // TODO: consider adding an indexer for item lookup.
    private readonly Dictionary<string, InventoryEntry> items = new();

    public IReadOnlyDictionary<string, InventoryEntry> Items => items;

    public bool Add(ItemData item)
    {
        if (item == null || string.IsNullOrWhiteSpace(item.Id))
            return false;
        
        // TODO: implement Max Stack
        // TODO: think how to store several stacks of same item
        
        if (items.TryGetValue(item.Id, out InventoryEntry entry))
        {
            entry.Quantity++;
        }
        else
        {
            if (items.Count >= Capacity)
                return false;
            
            entry = new();
            entry.ItemData = item;
            entry.Quantity = 1;

            items.Add(item.Id, entry);
        }
        
        return true;
    }
    
    public void Remove(ItemData itemData, int amount)
    {
        Console.WriteLine("Removing item " + itemData.Name);
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0)
            return;
        
        Money += amount;
    }

    public bool SpendMoney(int amount)
    {
        if (amount <= 0 || amount > Money)
            return false;

        Money -= amount;
        return true;
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