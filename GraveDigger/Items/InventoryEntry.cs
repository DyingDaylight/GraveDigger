namespace GraveDigger.Items;

public class InventoryEntry
{
    public ItemData ItemData { get; set; }
    public int Quantity { get; set; }

    public override string ToString()
    {
        return $"{ItemData} * {Quantity}";;
    }
}