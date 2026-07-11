using GraveDigger.Data;

namespace GraveDigger.Items;

public class LootGenerator
{

    public ItemData Generate(TombstoneData tombstone)
    {
        ItemData itemData = new ItemData();
        itemData.Id = "ring";
        itemData.Name = "Ring";
        itemData.Price = 20;
        return itemData;
    }
}