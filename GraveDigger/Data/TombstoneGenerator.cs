using System;
using System.Collections.Generic;
using GraveDigger.Utils;

namespace GraveDigger.Data;

public class TombstoneGenerator
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
    
    private static readonly int MinBirthYear = 1830;
    private static readonly int MaxBirthYear = 1910;
    
    private static readonly List<WeightedRange> AgeRanges =
    [
        new(18, 30, 15),
        new(31, 50, 25),
        new(51, 70, 35),
        new(71, 90, 25)
    ];
    
    private static readonly string[] WealthDescriptors =
        {"", "Slightly", "Quite", "Very", "Extremely", "Ridiculously"};

    private static readonly string[] PeacfulDescriptors =
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
    
    private static readonly Dictionary<TombPersonality, string[]> Inscriptions =
        new()
        {
            [TombPersonality.Peaceful] = PeacfulDescriptors,
            [TombPersonality.Restless] = RestlessDescriptors,
            [TombPersonality.Bitter] = BitterDescriptors,
            [TombPersonality.Greedy] = GreedyDescriptors,
            [TombPersonality.Cruel] = CruelDescriptors,
            [TombPersonality.Mysterious] = MysteriousDescriptors
        };
    
    public static TombstoneData GenerateTombstoneData(RandomService randomService)
    {
        TombstoneData tombstoneData = new TombstoneData();
        
        string firstName = randomService.Pick(FirstNames);
        string lastName = randomService.Pick(LastNames);
        tombstoneData.Name = $"{firstName} {lastName}";

        int birthYear = randomService.Next(MinBirthYear, MaxBirthYear);
        int age = randomService.PickWeightedRange(AgeRanges);
        int deathYear = birthYear + age;
        tombstoneData.Years = $"{birthYear} - {deathYear}";
        
        tombstoneData.WealthState = randomService.RandomEnum<TombWealth>();
        string wealthDescriptor = randomService.Pick(WealthDescriptors);
        tombstoneData.WealthDescription = $"{wealthDescriptor} {tombstoneData.WealthState}";
        
        tombstoneData.Personality = randomService.RandomEnum<TombPersonality>();
        tombstoneData.Inscription = randomService.Pick(Inscriptions[tombstoneData.Personality]);
        
        return tombstoneData;
    }
}