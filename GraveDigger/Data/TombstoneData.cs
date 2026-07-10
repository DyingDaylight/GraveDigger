namespace GraveDigger.Data;

public class TombstoneData
{
    public string Name;
    public string Years;
    public string Wealth;
    public string Nature;

    public TombstoneData(string name, string years, string wealth, string nature)
    {   
        Name = name;
        Years = years;
        Wealth = wealth;
        Nature = nature;
    }
}