using GraveDigger.Props;

namespace GraveDigger.Items;

public class BlueprintItemData : ItemData
{
    public DecorationType DecorationType { get; private set; }

    public string Product => DecorationType switch
    {
        DecorationType.FlowerBed => "Flower Bed",
        DecorationType.HouseUpgrade => "House Upgrade",
        _ => DecorationType.ToString()
    };
    
    public BlueprintItemData(string id, string name, 
        string description, string spriteName, int price, 
        int maxStackSize,
        DecorationType decorationType) : base(id, name, description, spriteName, price, maxStackSize)
    {
        DecorationType = decorationType;
    }
}