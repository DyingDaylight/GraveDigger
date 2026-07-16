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

    public event Action Changed;
    
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
        
        Changed?.Invoke();
        return true;
    }
    
    public bool Remove(ItemData itemData, int amount)
    {
        if (itemData == null || amount <= 0)
            return false;

        if (!items.TryGetValue(itemData.Id, out InventoryEntry entry))
            return false;

        int removedAmount = Math.Min(amount, entry.Quantity);

        entry.Quantity -= removedAmount;

        if (entry.Quantity == 0)
            items.Remove(itemData.Id);

        Changed?.Invoke();
        return true;
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0)
            return;
        
        Money += amount;
        Changed?.Invoke();
    }

    public bool SpendMoney(int amount)
    {
        if (amount <= 0 || amount > Money)
            return false;

        Money -= amount;
        Changed?.Invoke();
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