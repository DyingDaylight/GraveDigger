using System;
using System.Collections.Generic;
using System.Linq;
using GraveDigger.Data;
using GraveDigger.Utils;

namespace GraveDigger.Items;

public class LootGenerator
{

    private static readonly List<ItemData> pool = new()
{
    new ItemData("skull", "Human Skull", "Quite useless.", "Icon1", 1, 100),
    new ItemData("bone", "Old Bone", "Dry and brittle.", "Icon2", 1, 60),
    new ItemData("jawbone", "Jawbone", "Missing several teeth.", "Icon3", 1, 80),
    new ItemData("femur", "Femur Bone", "A large human bone.", "Icon4", 1, 75),
    new ItemData("hand", "Skeletal Hand", "Still strangely intact.", "Icon5", 1, 90),
    new ItemData("broken_bone", "Broken Bone", "Snapped clean in two.", "Icon6", 0, 65),
    new ItemData("ribcage", "Rib Cage", "The remains of a poor soul.", "Icon7", 1, 120),
    new ItemData("teeth", "Teeth", "A handful of old teeth.", "Icon8", 2, 40),

    new ItemData("fang", "Beast Fang", "Taken from a wild predator.", "Icon9", 2, 90),
    new ItemData("raven_skull", "Raven Skull", "An unsettling omen.", "Icon10", 3, 170),
    new ItemData("cracked_animal_skull", "Cracked Animal Skull", "Split long ago by time or violence.", "Icon11", 2, 110),
    new ItemData("heart", "Heart", "Still warm... somehow.", "Icon12", 3, 250),
    new ItemData("eyes", "Pair of Eyes", "Better not ask.", "Icon13", 4, 180),
    new ItemData("brain", "Brain", "Knowledge comes at a price.", "Icon14", 4, 220),
    new ItemData("severed_arm", "Severed Arm", "Cold and lifeless.", "Icon15", 2, 160),
    new ItemData("foot", "Severed Foot", "Not very useful anymore.", "Icon16", 2, 140),

    new ItemData("ringed_finger", "Ringed Finger", "The ring is worth far more than the finger.", "Icon17", 5, 450),
    new ItemData("silver_ring", "Silver Ring", "Tarnished with age.", "Icon18", 6, 220),
    new ItemData("ring", "Magic Ring", "A mysterious gemstone glows softly.", "Icon19", 7, 600),
    new ItemData("crown", "Golden Crown", "Fit for forgotten royalty.", "Icon20", 10, 1000),
    new ItemData("potion", "Potion", "A cloudy alchemical brew.", "Icon21", 6, 180),
    new ItemData("moneybag", "Coin Pouch", "Contains a few old coins.", "Icon22", 4, 350),
    new ItemData("stakes", "Wooden Stakes", "Sharpened stakes, ready to keep something buried.", "Icon23", 4, 80),
    new ItemData("sealed_scroll", "Sealed Scroll", "A tightly rolled parchment sealed with wax.", "Icon24", 6, 350),

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
};

    private static readonly Dictionary<TombWealth, (int Min, int Max)> wealthRanges = new()
    {
        [TombWealth.Poor] = (0, 3),
        [TombWealth.Average] = (2, 5),
        [TombWealth.Rich] = (3, 8),
        [TombWealth.Wealthy] = (6, 10)
    };
    
    private static readonly Dictionary<TombWealth, (int Min, int Max)> amountRanges = new()
    {
        [TombWealth.Poor] = (0, 1),
        [TombWealth.Average] = (0, 2),
        [TombWealth.Rich] = (1, 3),
        [TombWealth.Wealthy] = (0, 3)
    };
    
    
    public List<ItemData> Generate(TombstoneData tombstone, RandomService random)
    {
        var wealthRange = wealthRanges[tombstone.WealthState];
        int budget = random.Next(wealthRange.Min, wealthRange.Max + 1);
        
        var amountRange = amountRanges[tombstone.WealthState];
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
}