namespace GraveDigger.Data;

public class TombstoneData
{
    public string Name;
    public string Years;
    public string Wealth;
    public string Nature;
    public string State;

    public TombstoneData(string name, string years, string wealth, string nature, string state)
    {   
        Name = name;
        Years = years;
        Wealth = wealth;
        Nature = nature;
        State = state;
    }
}