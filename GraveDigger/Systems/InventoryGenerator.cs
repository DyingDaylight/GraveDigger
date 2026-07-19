using System.Collections.Generic;
using GraveDigger.Items;
using GraveDigger.Utils;

namespace GraveDigger.Systems;

public static class InventoryGenerator
{
    private static readonly List<FoodItemData> foodPool = new()
    {
        new FoodItemData("bread",      "Village Bread",   "Freshly baked and wonderfully filling.",      "Food1",  6, 10, 20),
        new FoodItemData("stew",       "Hearty Stew",     "A warm bowl of meat and vegetables.",          "Food2", 18, 10, 50),
        new FoodItemData("cheese",     "Aged Cheese",     "Rich flavor with a pleasantly sharp taste.",   "Food3", 14, 10, 30),
        new FoodItemData("jerky",      "Dried Meat",      "Salted meat that lasts for weeks.",            "Food4", 16, 10, 40),
        new FoodItemData("apple",      "Red Apple",       "Sweet, juicy and freshly picked.",             "Food5",  5, 10, 15),
        new FoodItemData("pie",        "Berry Pie",       "A homemade pie packed with forest berries.",   "Food6", 20, 10, 45),
        new FoodItemData("milk",       "Fresh Milk",      "A bottle of creamy fresh milk.",               "Food7",  8, 10, 25),
        new FoodItemData("mushrooms",  "Roasted Mushrooms","Earthy mushrooms cooked over a fire.",        "Food8", 12, 10, 35),
        new FoodItemData("fish",       "Dried Fish",      "Simple but nourishing trail food.",            "Food9", 15, 10, 35),
        new FoodItemData("honey",      "Jar of Honey",    "Golden honey with a naturally sweet taste.",   "Food10",22, 10, 30),
    };
    
    public static ItemData GetRandomFood(RandomService randomService)
    {
        return randomService.Pick(foodPool);
    }

    public static Inventory CreateInventory(RandomService randomService)
    {
        // TODO: make more reasonable generation
        Inventory inventory = new Inventory();
        inventory.AddMoney(100);
        inventory.Add(GetRandomFood(randomService));
        inventory.Add(GetRandomFood(randomService));
        inventory.Add(GetRandomFood(randomService));
        inventory.Add(GetRandomFood(randomService));
        return inventory;
    }
}