using System;
using System.Collections.Generic;
using System.Linq;
using GraveDigger.Data;
using GraveDigger.Items;
using GraveDigger.Utils;

namespace GraveDigger.Systems;

public class LootGenerator
{

    private static readonly List<ItemData> pool = new()
{
    new ItemData("helmet", "Knight helmet", "Quite useless", "Icon1", 1, 100),
    new ItemData("map", "Treasure map", "The treasure is not for sale!", "Icon2", 1, 60),
    new ItemData("quiver1", "Quiver", "From the Elves of Middle-earth", "Icon3", 1, 80),
    new ItemData("vial1", "Vial of wild brew", "One more for the road?", "Icon4", 1, 75),
    new ItemData("skull1", "Wild buffalo skull", "Scary, but it won't butt...", "Icon5", 1, 90),
    new ItemData("crown", "Crown of the Last Silk King", "Smells like a ragdoll dragon", "Icon6", 0, 65),
    new ItemData("necklace", "Knitted necklace", "Even Grandma knitted it", "Icon7", 1, 120),
    new ItemData("ring1", "'My precious'", "Gorlum or Gollum?", "Icon8", 2, 40),

    new ItemData("ring2", "Magical woolen ring", "It radiates magic", "Icon9", 2, 90),
    new ItemData("book", "Book of Edible Dishes", "Care for a bite, traveler?", "Icon10", 3, 170),
    new ItemData("bag", "Bag of buttons", "Not enough for a life of luxury, but enough for a feast - why not?", "Icon11", 2, 110),
    new ItemData("vial2", "Vial of glue", "Bottle of water? Not, glue..", "Icon12", 3, 250),
    new ItemData("skein1", "Skein of burgundy yarn", "You've run me ragged!", "Icon13", 4, 180),
    new ItemData("skein2", "Skein of royal yarn", "Remember my dress from the latest collection? That's the one!", "Icon14", 4, 220),
    new ItemData("lamb", "Little Lamb \"Baa\"", "Very soft, but cursed", "Icon15", 2, 160),
    new ItemData("bat", "Ancient bat", "They were knitted in the dark", "Icon16", 2, 140),

    new ItemData("skull2", "Knit human skull", "Didn't come unraveled in the dirt", "Icon17", 5, 450),
    new ItemData("gem", "Precious gem \"The Knitstone\"", "A couple of knitted gnomes gave their lives for it", "Icon18", 6, 220),
    new ItemData("vial3", "Roll of multicolored fabric", "Simply tasteless..", "Icon19", 7, 600),
    new ItemData("spider", "Little spider", "He is definitely spinning his little web", "Icon20", 10, 1000),
    new ItemData("quiver2", "Elven quiver", "It never belonged to any Elf", "Icon21", 6, 180),
    new ItemData("glove", "Duelist's glove", "Monsieur, I believe you dropped this accidentally", "Icon22", 4, 350),
    new ItemData("pouch", "Knitting tool pouch", "Knit me if you can!", "Icon23", 4, 80),
    /* new ItemData("sealed_scroll", "Sealed Scroll", "A tightly rolled parchment sealed with wax.", "Icon24", 6, 350),

    new ItemData("spellbook", "Spellbook", "Filled with forgotten rituals.", "Icon25", 9, 700),
    new ItemData("garlic_garland", "Garlic Garland", "The smell alone could repel the undead.", "Icon26", 3, 140),
    new ItemData("raven_feet", "Raven's Feet", "Dried and tied together with twine.", "Icon27", 2, 140),
    new ItemData("gem", "Blue Gem", "A precious crystal.", "Icon28", 8, 500),
    new ItemData("ruby", "Ruby", "Deep crimson and valuable.", "Icon29", 10, 800),
    new ItemData("locket", "Silver Locket", "An old family portrait is hidden inside.", "Icon30", 5, 260),
    new ItemData("dagger", "Dagger", "Short but deadly.", "Icon31", 4, 350),
    new ItemData("broken_axe", "Broken Axe", "Its handle snapped long ago.", "Icon32", 0, 180),

    new ItemData("skull_candle", "Skull Candle", "A candle burns atop a weathered human skull.", "Icon33", 3, 180),
    new ItemData("candles", "Pair of Candles", "Still usable despite their age.", "Icon34", 2, 70),
    new ItemData("letter", "Old Letter", "Its ink has almost faded.", "Icon35", 3, 180),
    new ItemData("vial", "Blood Vial", "Fresh enough to worry.", "Icon36", 6, 240),
    new ItemData("broom", "Old Broom", "Useful for sweeping leaves... or hiding tracks.", "Icon37", 2, 50),
    new ItemData("voodoo_doll", "Voodoo Doll", "Bound with twine and stuck with pins.", "Icon38", 7, 320),
    new ItemData("buttons", "Buttons", "Recovered from an old burial garment.", "Icon39", 2, 30),

    new ItemData("old_key", "Old Key", "Perhaps it still opens something.", "Icon41", 8, 180),
    new ItemData("letter_sealed", "Sealed Letter", "The wax seal is unbroken.", "Icon42", 4, 220),
    new ItemData("scroll", "Ancient Scroll", "Covered in faded runes.", "Icon43", 8, 450),
    new ItemData("ceremonial_helmet", "Ceremonial Helmet", "Decorated with a pair of horns.", "Icon44", 9, 500),
    new ItemData("gloves", "Leather Gloves", "Still usable.", "Icon45", 5, 50),
    new ItemData("boots", "Leather Boots", "Well worn but usable.", "Icon46", 7, 140),
    new ItemData("broken_arrow", "Broken Arrow", "Snapped in battle long ago.", "Icon47", 0, 50),
    new ItemData("deer_skull", "Deer Skull", "A woodland trophy.", "Icon48", 4, 180),
    
    */
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