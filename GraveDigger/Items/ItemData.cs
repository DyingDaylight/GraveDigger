namespace GraveDigger.Items;

public abstract class ItemData
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public string SpriteName { get; }
    public int Price { get; }
    public int MaxStackSize { get; }

    public ItemData(string id, string name, string description, string spriteName, int price, int maxStackSize)
    {
        Id = id;
        Name = name;
        Description = description;
        SpriteName = spriteName;
        Price = price;
        MaxStackSize = maxStackSize;
    }
    
    public override string ToString()
    {
        return Name + "\n" + Description;
    }
}