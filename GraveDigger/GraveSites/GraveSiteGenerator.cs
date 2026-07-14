using System.Collections.Generic;
using GraveDigger.Utils;

namespace GraveDigger.Data;

public class GraveSiteGenerator
{
    private static readonly string[] FirstNames =
    {
        "Eleanor",
        "Abigail",
        "Edith",
        "Agnes",
        "Margaret",
        "Charlotte",
        "Beatrice",
        "Rose",
        "Florence",
        "Alice",
        "Mary",
        "Catherine",
        "Amelia",
        "Evelyn",
        "Clara",
        "Henry",
        "William",
        "Edward",
        "Thomas",
        "Arthur",
        "George",
        "Charles",
        "Frederick",
        "Samuel",
        "Benjamin",
        "Walter",
        "Alfred",
        "Percival",
        "Edmund",
        "Victor"
    };
    
    private static readonly string[] LastNames =
    {
        "Blackwood",
        "Ashcroft",
        "Ravenscroft",
        "Whitmore",
        "Graves",
        "Hawthorne",
        "Wellington",
        "Sinclair",
        "Fairchild",
        "Lockwood",
        "Pembroke",
        "Winters",
        "Holloway",
        "Thorne",
        "Crowley",
        "Redgrave",
        "Morrigan",
        "Briar",
        "Sterling",
        "Harrington",
        "Kingsley",
        "Winchester",
        "Foxworth",
        "Ashby",
        "Bellamy",
        "Carrington",
        "Drake",
        "Everhart",
        "Langley",
        "Mortimer"
    };
    
    private const int MinBirthYear = 1830;
    private const int MaxBirthYear = 1910;
    
    private static readonly List<WeightedRange> AgeRanges =
    [
        new(18, 30, 15),
        new(31, 50, 25),
        new(51, 70, 35),
        new(71, 90, 25)
    ];
    
    private static readonly string[] WealthDescriptors =
        {"", "Slightly", "Quite", "Very", "Extremely", "Ridiculously"};

    private static readonly string[] PeacefulDescriptors =
    {
        "Kind Soul", "Gentle Spirit", "Beloved Neighbor", "Warm Heart", "Compassionate Person"
    };
    private static readonly string[] RestlessDescriptors =
    {
        "Never at Peace", "Wandering Spirit", "Troubled Soul", "Could Not Rest"
    };
    private static readonly string[] BitterDescriptors =
    {
        "Grumpy Old Soul", "Cantankerous Fellow", "Ill-Tempered Neighbor", "Sour Old Hermit"
    };
    private static readonly string[] GreedyDescriptors =
    {
        "Miser", "Treasure Hoarder", "Gold Lover", "Fortune Hunter", "Avaricious Soul"
    };
    private static readonly string[] CruelDescriptors = 
    { 
        "Cold-Hearted", "Merciless Master", "Feared Landowner", "Ruthless Soul" 
    };
    private static readonly string[] MysteriousDescriptors =
    {
        "Village Oddity", "Outsider", "Peculiar Soul", "Strange Dreamer"
    };
    
    private static readonly Dictionary<Personality, string[]> Inscriptions =
        new()
        {
            [Personality.Peaceful] = PeacefulDescriptors,
            [Personality.Restless] = RestlessDescriptors,
            [Personality.Bitter] = BitterDescriptors,
            [Personality.Greedy] = GreedyDescriptors,
            [Personality.Cruel] = CruelDescriptors,
            [Personality.Mysterious] = MysteriousDescriptors
        };
    
    public static GraveSiteData Generate(RandomService randomService)
    {
        GraveSiteData graveSiteData = new GraveSiteData();
        
        string firstName = randomService.Pick(FirstNames);
        string lastName = randomService.Pick(LastNames);
        graveSiteData.Name = $"{firstName} {lastName}";

        int birthYear = randomService.Next(MinBirthYear, MaxBirthYear + 1);
        int age = randomService.PickWeightedRange(AgeRanges);
        int deathYear = birthYear + age;
        graveSiteData.LifeYears = $"{birthYear} - {deathYear}";
        
        graveSiteData.Wealth = randomService.RandomEnum<Wealth>();
        string wealthDescriptor = randomService.Pick(WealthDescriptors);
        graveSiteData.WealthDescription = string.IsNullOrWhiteSpace(wealthDescriptor)
            ? graveSiteData.Wealth.ToString()
            : $"{wealthDescriptor} {graveSiteData.Wealth}";
        
        graveSiteData.Personality = randomService.RandomEnum<Personality>();
        graveSiteData.Inscription = randomService.Pick(Inscriptions[graveSiteData.Personality]);
        
        return graveSiteData;
    }
}