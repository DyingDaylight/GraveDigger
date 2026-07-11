using GraveDigger.Props;

namespace GraveDigger.Items;

public class ItemData
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string SpriteName { get; set; }
    public int Price { get; set; }
    public int MaxStackSize { get; set; }

    public override string ToString()
    {
        return Name;
    }
}