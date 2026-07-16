namespace GraveDigger.Items;

public class FoodItemData : ItemData
{
    public int Nutrition { get; }
    
    public FoodItemData(string id, string name, string description, 
        string spriteName, int price, int maxStackSize,
        int nutrition) 
        : base(id, name, description, spriteName, price, maxStackSize)
    {
        Nutrition = nutrition;
    }
    
    public override string ToString()
    {
        return $"{Name} (-{Nutrition} Hunger)\n{Description}";
    }
}