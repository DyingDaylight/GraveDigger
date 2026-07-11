namespace GraveDigger.Data;

public class TombstoneData
{
    public string Name;
    public string Years;
    public TombWealth WealthState;
    public string WealthDescription;
    public TombPersonality Personality;
    public string PersonalityDescription;
    public string Epitaph;

    public TombstoneData()
    {
        
    }
    
    public TombstoneData(string name, string years, TombWealth wealth, string personality)
    {   
        Name = name;
        Years = years;
        WealthState = wealth;
        PersonalityDescription = personality;
    }

}