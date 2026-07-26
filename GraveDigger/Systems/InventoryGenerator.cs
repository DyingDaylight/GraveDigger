using System;
using System.Collections.Generic;
using System.Linq;
using GraveDigger.Items;
using GraveDigger.Props;
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

    private static readonly List<BlueprintItemData> blueprintsPool = new()
    {
        new BlueprintItemData(
            "bench_blueprint",
            "Bench Blueprint",
            "A quiet place where visitors can sit and remember.",
            "benchIcon",
            25,
            1,
            DecorationType.Bench),

        new BlueprintItemData(
            "flowerbed_blueprint",
            "Flower Bed Blueprint",
            "A splash of color among the weathered stones.",
            "flowerbedIcon",
            20,
            1,
            DecorationType.FlowerBed),

        new BlueprintItemData(
            "lamppost_blueprint",
            "Lamppost Blueprint",
            "A warm light to guide visitors after sunset.",
            "lampostIcon",
            35,
            1,
            DecorationType.Lamp),

        new BlueprintItemData(
            "fence_blueprint",
            "Fence Blueprint",
            "A gentle boundary for all that has been entrusted to our care.",
            "fenceIcon",
            30,
            1,
            DecorationType.Fence),

        new BlueprintItemData(
            "tree_blueprint",
            "Tree Blueprint",
            "A young tree that will watch over the cemetery for years to come.",
            "treeIcon",
            40,
            1,
            DecorationType.Tree),

        new BlueprintItemData(
            "house_upgrade_blueprint",
            "House Upgrade Blueprint",
            "Every caretaker deserves a place that feels like home.",
            "houseIcon",
            80,
            1,
            DecorationType.HouseUpgrade)
    };
    
    public static Inventory CreatePlayerInventory(RandomService randomService)
    {
        Inventory inventory = new Inventory();

        inventory.AddMoney(50);

        inventory.Add(GetRandomFood(randomService));
        inventory.Add(GetRandomFood(randomService));
        inventory.Add(GetRandomFood(randomService));

        inventory.Add(blueprintsPool[2], 5);
        inventory.Add(blueprintsPool[1], 5);
        
        return inventory;
    }
    
    public static Inventory CreateMerchantInventory(RandomService randomService,
        Func<DecorationType, bool> hasLockedDecorations)
    {
        Inventory inventory = new Inventory();
        inventory.AddMoney(100);

        AddMerchantItems(inventory, randomService, hasLockedDecorations);

        return inventory;
    }
    
    public static void AddMerchantItems(Inventory inventory,
        RandomService randomService, Func<DecorationType, bool> hasLockedDecorations)
    {
        int delta = randomService.Next(-20, 41);
        int newSum = inventory.Money + delta;
        newSum = Math.Clamp(newSum, 50, 300);
        delta = newSum - inventory.Money;
        inventory.AddMoney(delta);
        
        inventory.Add(GetRandomFood(randomService));
        inventory.Add(GetRandomFood(randomService));
        inventory.Add(GetRandomFood(randomService));

        AddRandomBlueprints(inventory, randomService,
            hasLockedDecorations, 2);
    }
    
    public static Inventory CreateTestInventory()
    {
        Inventory inventory = new Inventory();

        inventory.AddMoney(999);

        inventory.Add(foodPool[0], 1);
        inventory.Add(foodPool[1], 3);
        inventory.Add(foodPool[2], 10);

        inventory.Add(blueprintsPool[0], 1);
        inventory.Add(blueprintsPool[1], 2);
        inventory.Add(blueprintsPool[2], 2);
        inventory.Add(blueprintsPool[3], 2);
        inventory.Add(blueprintsPool[4], 2);
        inventory.Add(blueprintsPool[5], 2);

        return inventory;
    }
    
    private static void AddRandomBlueprints(
        Inventory inventory, RandomService randomService,
        Func<DecorationType, bool> hasLockedDecorations, int count)
    {
        List<BlueprintItemData> availableBlueprints = blueprintsPool
            .Where(blueprint => hasLockedDecorations(blueprint.DecorationType))
            .ToList();

        for (int i = 0; i < count && availableBlueprints.Count > 0; i++)
        {
            BlueprintItemData blueprint = randomService.Pick(availableBlueprints);

            inventory.Add(blueprint);
            availableBlueprints.Remove(blueprint);
        }
    }
    
    private static ItemData GetRandomFood(RandomService randomService)
    {
        return randomService.Pick(foodPool);
    }
}