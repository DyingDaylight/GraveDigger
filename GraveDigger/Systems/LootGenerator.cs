using System;
using System.Collections.Generic;
using System.Linq;
using GraveDigger.Data;
using GraveDigger.Items;
using GraveDigger.Utils;

namespace GraveDigger.Systems;

public class LootGenerator
{

    private static readonly List<LootItemData> pool = new()
{
    new LootItemData("helmet", "Knight helmet", "Quite useless", "Icon1", 1, 100),
    new LootItemData("map", "Treasure map", "The treasure is not for sale!", "Icon2", 1, 60),
    new LootItemData("quiver1", "Quiver", "From the Elves of Middle-earth", "Icon3", 1, 80),
    new LootItemData("vial1", "Vial of wild brew", "One more for the road?", "Icon4", 1, 75),
    new LootItemData("skull1", "Wild buffalo skull", "Scary, but it won't butt...", "Icon5", 1, 90),
    new LootItemData("crown", "Crown of the Last Silk King", "Smells like a ragdoll dragon", "Icon6", 0, 65),
    new LootItemData("necklace", "Knitted necklace", "Even Grandma knitted it", "Icon7", 1, 120),
    new LootItemData("ring1", "'My precious'", "Gorlum or Gollum?", "Icon8", 2, 40),

    new LootItemData("ring2", "Magical woolen ring", "It radiates magic", "Icon9", 2, 90),
    new LootItemData("book", "Book of Edible Dishes", "Care for a bite, traveler?", "Icon10", 3, 170),
    new LootItemData("bag", "Bag of buttons", "Not enough for a life of luxury, but enough for a feast - why not?", "Icon11", 2, 110),
    new LootItemData("vial2", "Vial of glue", "Bottle of water? Not, glue..", "Icon12", 3, 250),
    new LootItemData("skein1", "Skein of burgundy yarn", "You've run me ragged!", "Icon13", 4, 180),
    new LootItemData("skein2", "Skein of royal yarn", "Remember my dress from the latest collection? That's the one!", "Icon14", 4, 220),
    new LootItemData("lamb", "Little Lamb \"Baa\"", "Very soft, but cursed", "Icon15", 2, 160),
    new LootItemData("bat", "Ancient bat", "They were knitted in the dark", "Icon16", 2, 140),

    new LootItemData("skull2", "Knit human skull", "Didn't come unraveled in the dirt", "Icon17", 5, 450),
    new LootItemData("gem", "Precious gem \"The Knitstone\"", "A couple of knitted gnomes gave their lives for it", "Icon18", 6, 220),
    new LootItemData("vial3", "Roll of multicolored fabric", "Simply tasteless..", "Icon19", 7, 600),
    new LootItemData("spider", "Little spider", "He is definitely spinning his little web", "Icon20", 10, 1000),
    new LootItemData("quiver2", "Elven quiver", "It never belonged to any Elf", "Icon21", 6, 180),
    new LootItemData("glove", "Duelist's glove", "Monsieur, I believe you dropped this accidentally", "Icon22", 4, 350),
    new LootItemData("pouch", "Knitting tool pouch", "Knit me if you can!", "Icon23", 4, 80),
};

    private static readonly Dictionary<Wealth, (int Min, int Max)> BudgetRangesByWealth = new()
    {
        [Wealth.Poor] = (0, 3),
        [Wealth.Average] = (2, 5),
        [Wealth.Rich] = (3, 8),
        [Wealth.Wealthy] = (6, 10)
    };
    
    private static readonly Dictionary<Wealth, (int Min, int Max)> AmountRangesByWealth = new()
    {
        [Wealth.Poor] = (0, 1),
        [Wealth.Average] = (0, 2),
        [Wealth.Rich] = (1, 3),
        [Wealth.Wealthy] = (0, 3)
    };
    
    
    public List<ItemData> Generate(GraveSiteData graveSite, RandomService random)
    {
        var wealthRange = BudgetRangesByWealth[graveSite.Wealth];
        int budget = random.Next(wealthRange.Min, wealthRange.Max + 1);
        
        var amountRange = AmountRangesByWealth[graveSite.Wealth];
        int amount = random.Next(amountRange.Min, amountRange.Max + 1);

        List<ItemData> lootItems = new();
        
        if (amount == 0 || pool.Count == 0)
            return lootItems;
        
        List<ItemData> availableItems = new(pool);
        
        while (amount > 0 && availableItems.Count > 0)
        {
            int targetValue = budget / amount;

            int min = Math.Max(0, targetValue - 2);
            int max = targetValue + 2;

            List<ItemData> candidates = availableItems
                .Where(item => item.Price >= min &&
                               item.Price <= max)
                .ToList();

            // If no items fall within the target price range,
            // select the closest-priced item instead.
            if (candidates.Count == 0)
            {
                candidates = availableItems
                    .OrderBy(item => Math.Abs(item.Price - targetValue))
                    .Take(1)
                    .ToList();
            }

            ItemData item = random.Pick(candidates);

            lootItems.Add(item);
            availableItems.Remove(item);

            budget = Math.Max(0, budget - item.Price);
            amount--;
        }
        
        return lootItems;
    }

    public ItemData GetRandomItem(RandomService random)
    {
        return random.Pick(pool);
    }
}