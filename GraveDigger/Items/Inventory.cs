using System.Collections.Generic;
using System.Text;

namespace GraveDigger.Items;

public class Inventory
{
    private Dictionary<string, InventoryEntry> items = new();

    public bool Add(ItemData item)
    {
        // TODO: implement Max Stack
        // TODO: think how to store several stacks of same item
        if (items.ContainsKey(item.Id))
        {
            items[item.Id].Quantity++;
        }
        else
        {
            InventoryEntry entry = new();
            entry.ItemData = item;
            entry.Quantity = 1;
            items[entry.ItemData.Id] = entry;
        }
        return true;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine("Inventory: ");
        foreach (InventoryEntry entry in items.Values)
        {
            sb.AppendLine(entry.ToString());
        }
        return sb.ToString();
    }
}