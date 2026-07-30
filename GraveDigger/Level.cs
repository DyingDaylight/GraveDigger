
using System;
using System.Collections.Generic;
using System.Linq;
using GraveDigger.Characters;
using GraveDigger.Core;
using GraveDigger.Data;
using GraveDigger.Enemies;
using GraveDigger.GraveSites;
using GraveDigger.Interactions;
using GraveDigger.Items;
using GraveDigger.Props;
using GraveDigger.Systems;
using GraveDigger.Utils;
using GraveDigger.Visuals;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GraveDigger;

public class Level : IUpdatable, IDrawable
{
    private const int MaxGhosts = 5;
    private const int HungerPerDay = 15;
    private const int InitialHunger = 20;
    private const int MapTileTypesCount = 3;
    private readonly int[,] tileMapSchema =
    {
        { 1, 0, 0, 1, 1, },
        { 0, 2, 2, 0, 0, },
        { 0, 2, 2, 0, 0, },
        { 1, 2, 2, 1, 1, },
    };
    
    private readonly List<Prop> props = new();
    private readonly Map map = new();
    
    private readonly List<IUpdatable> updatables = new();
    private readonly List<IDrawable> drawables = new();
    private readonly List<Collider> colliders = new();
    private readonly List<IReputationContributor> contributors = new();
    private readonly List<ILightSource> lightSources = new();

    private readonly GameContext gameContext;
    
    private readonly PlayerTrail playerTrail;
    
    private readonly List<Decoration> decorations = new();
    private readonly List<GraveSite> graveSites = new();
    private readonly List<Ghost> ghosts = new();
    
    private readonly DayNightOverlay dayNightOverlay;

    private bool isNight = false;
    
    private Merchant merchant;
    public Player Player { get; private set; }

    public InteractionSystem InteractionSystem { get; }
    public IReadOnlyList<GraveSite> GraveSites => graveSites;

    public event Action ReputationRecalculationRequested;
    public event Action<GraveSite> GraveInteractionRequested;
    public event Action<Merchant> MarketOpenRequested;
    public event Action<ItemPickUp> ItemPickupRequested;
    public event Action<GraveSite> GraveOccupied;
    
    public Level(GameContext gameContext, DayNightOverlay dayNightOverlay)
    {
        this.gameContext = gameContext;
        this.dayNightOverlay = dayNightOverlay;
        
        InteractionSystem = new InteractionSystem(gameContext.CoordinatesConverter);
        playerTrail = new PlayerTrail();
    }
    
    public void LoadContent()
    {
        LoadGroundTextures();
        LoadPropTextures();
        LoadTombstoneTextures();
        LoadDecorationTextures();
        LoadLootTextures();
        LoadFoodTextures();
        LoadDecorationIconsTextures();
        LoadCharacterTextures();
    }
    
    public void Start()
    { 
        CreateMap();
        CreateProps();
        CreateGraveSites();
        CreateDecorations();
        
        CreateMerchant();
        
        Player = CreateLevelCharacter<Player>();
        Player.Transform.Position = new Vector2(746, 3463);
        Player.SetWorldSize(gameContext.WorldSize);
        Player.IncreaseHunger(InitialHunger);
        playerTrail.Record(Player.Transform.Position);

        foreach (IUpdatable updatable in updatables)
            updatable.Start();
        
        ReputationRecalculationRequested?.Invoke();
    }

    public void Update(GameTime gameTime)
    {
        map.Update(gameTime);

        for (int i = 0; i < ghosts.Count; i++)
        {
            float offsetX = i % 2 == 0 ? -12f : 12f;
            float offsetY = i % 2 == 0 ? -12f : 12f;
            
            ghosts[i].TargetPosition = playerTrail.GetFollowerPosition(i) 
                                       + new Vector2(offsetX, offsetY);
        }

        foreach (IUpdatable updatable in updatables)
            updatable.Update(gameTime);
        
        foreach (Collider collider in colliders)
            collider.Update(gameTime);
        
        InteractionSystem.Update(gameTime);
        playerTrail.Record(Player.Transform.Position);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        map.Draw(spriteBatch);
        foreach (IDrawable drawable in drawables)
            drawable.Draw(spriteBatch);
        
        foreach (Collider collider in colliders)
            collider.Draw(spriteBatch);
    }
    
    public void DrawLights(SpriteBatch spriteBatch)
    {
        if (!isNight)
            return;
        
        foreach (ILightSource lightSource in lightSources)
        {
            lightSource.DrawLight(spriteBatch);
        }
    }
    
    public void DrawOverlay(SpriteBatch spriteBatch)
    {
        dayNightOverlay.Draw(spriteBatch);
    }
    
    public void DayTimeChange(DayTime dayTime)
    {
        if (dayTime == DayTime.Day)
        {
            Console.WriteLine("++ Day started ++");
            merchant.RefreshInventory(gameContext.RandomService, HasBlueprintTarget);
            merchant.ChangeState(MerchantState.Arriving);
            isNight = false;
        } 
        else if (dayTime == DayTime.Night)
        {
            Console.WriteLine("++ Night started ++");
            merchant.ChangeState(MerchantState.Leaving);
            isNight = true;
        }
    }

    public void DayStart(int day)
    {
        Player.IncreaseHunger(HungerPerDay);
        ReputationRecalculationRequested?.Invoke();
    }

    public void SpawnLoot(List<ItemData> loot, Tombstone tombstone)
    {
        List<Rectangle> occupiedAreas = props.Where(prop => prop.Visible)
            .Select(prop => prop.GetDestRectangle(prop.SourceRectangle))
            .ToList();

        foreach (ItemData item in loot)
        {
            ItemPickUp itemPickUp = CreatePickupItem(tombstone, item, occupiedAreas);
            if (itemPickUp != null)
                itemPickUp.Start();
        }
    }
    
    public void SpawnUndead(EnemyType enemyType, GraveSite graveSite)
    {
        switch (enemyType)
        {
            case EnemyType.Ghost:
                bool isCreated = CreateGhost(graveSite.Transform.Position);
                if (isCreated)
                {
                    AudioManager.Instance.PlaySFX("ghost-spawn");
                    ReputationRecalculationRequested?.Invoke();
                }
                break;
        }
    }
    
    public bool BuildDecoration(BlueprintItemData blueprint)
    {
        Decoration decoration = decorations.FirstOrDefault(
            d => d.DecorationType == blueprint.DecorationType &&
                 d.CanApplyBlueprint);

        if (decoration == null)
            return false;

        return decoration.ApplyBlueprint();
    }
    
    public void RemovePickup(ItemPickUp pickable)
    {
        if (!props.Remove(pickable))
            return;

        InteractionSystem.UnregisterInteraction(pickable.Interaction);
        UnregisterObject(pickable);
    }
    
    public void DecreaseHunger(int nutritionAmount)
    {
        Player.DecreaseHunger(nutritionAmount);
    }

    public IEnumerable<IReputationContributor> GetReputationContributors()
    {
        return contributors;
    }
    
    private T RegisterObject<T>(T obj)
    {
        if (obj is IUpdatable updatable && !updatables.Contains(updatable))
            updatables.Add(updatable);

        if (obj is IDrawable drawable && !drawables.Contains(drawable))
            drawables.Add(drawable);

        if (obj is IHasCollider hasCollider && hasCollider.Collider != null 
                                            && !colliders.Contains(hasCollider.Collider))
            colliders.Add(hasCollider.Collider);
        
        if (obj is IReputationContributor contributor && !contributors.Contains(contributor))
            contributors.Add(contributor);
        
        if (obj is Decoration decoration && !decorations.Contains(decoration))
            decorations.Add(decoration);

        if (obj is ILightSource lightSource && !lightSources.Contains(lightSource))
            lightSources.Add(lightSource);
        
        return obj;
    }

    private void UnregisterObject<T>(T obj)
    {
        if (obj is IUpdatable updatable)
            updatables.Remove(updatable);

        if (obj is IDrawable drawable)
            drawables.Remove(drawable);

        if (obj is IHasCollider hasCollider && hasCollider.Collider != null)
            colliders.Remove(hasCollider.Collider);
        
        if (obj is IReputationContributor contributor)
            contributors.Remove(contributor);
        
        if (obj is Decoration decoration)
            decorations.Remove(decoration);

        if (obj is ILightSource lightSource)
            lightSources.Remove(lightSource);
    }
    
    private void CreateMap()
    {
        map.TileMap = tileMapSchema;
        map.Start();

        Vector2[] roadTiles =
        {
            // height - 294
            // From House Up
            new Vector2(1133, 3717),
            new Vector2(1133, 3423),
            new Vector2(1133, 3129),
            new Vector2(1133, 2835),

            // House <--> Merchant
            new Vector2(1413, 2835),

            // From Merchant Up
            new Vector2(3093, 3717),
            new Vector2(3093, 3423),
            new Vector2(3093, 3129),
            new Vector2(3093, 2835),
            
            // Center Up
            new Vector2(2093, 2870),
            new Vector2(2093, 2576),
            new Vector2(2093, 2282),
            new Vector2(2093, 1988),
            new Vector2(2093, 1694),
        };
        

        foreach (Vector2 roadTile in roadTiles)
        {
            Prop road = CreateLevelObject(PropFactory, $"road{gameContext.RandomService.Next(1,4)}", roadTile);
            road.Transform.Scale = new Vector2(1f, 1f);
            road.Mode = SortingMode.Fixed;
            road.CastShadow = false;
        }
    }

    private T CreateLevelCharacter<T>() where T : new()
    {
        T levelCharacter = new T();
        RegisterObject(levelCharacter);
        return  levelCharacter;
    }
    
    private T CreateLevelObject<T>(Func<string, T> factory, string name, Vector2 position) where T : Prop
    {
        T prop = factory(name);
        prop.Transform.Position = position;
        prop.CastShadow = true;
        switch (name)
        {
            case "lampost":
                prop.Transform.Scale = new Vector2(0.09f, 0.09f);
                break;

            case "dirt":
                prop.Transform.Scale = new Vector2(0.05f, 0.05f);
                break;
    
            case "crypt":
                prop.Transform.Scale = new Vector2(0.18f, 0.18f);
                break;
    
            case "grave_earth":
                prop.Transform.Scale = new Vector2(0.08f, 0.08f);
                break;
    
            case "grave_digged":
                prop.Transform.Scale = new Vector2(0.08f, 0.08f);
                break;
            
            case "tree":
                prop.Transform.Scale = new Vector2(1f, 1f);
                break;

            default:
                prop.Transform.Scale = new Vector2(0.3f, 0.3f);
                break;
        }
        if (prop is Decoration decoration)
            InteractionSystem.RegisterInteraction(decoration.HintInteraction);
        RegisterObject(prop);
        props.Add(prop);
        return prop;
    }
    
    private ItemPickUp CreatePickupItem(Prop sourceProp, ItemData item, List<Rectangle> occupiedAreas)
    {
        ItemPickUp itemPickUp = CreateLevelObject(LootFactory, item.SpriteName, sourceProp.Transform.Position);
        itemPickUp.Transform.Scale = new Vector2(0.3f, 0.3f);
        itemPickUp.SetData(item);
        
        Vector2 origin = new Vector2(sourceProp.Left + sourceProp.Width * 0.5f, sourceProp.Bottom);

        Point itemSize = new(
            itemPickUp.DestRectangle.Width,
            itemPickUp.DestRectangle.Height
        );
        
        LootPlacementService.LootPlacement? placement =
            LootPlacementService.FindFreePosition(origin, itemSize, occupiedAreas);

        if (!placement.HasValue)
        {
            RemovePickup(itemPickUp);
            return null;
        }

        itemPickUp.Transform.Position = placement.Value.Position;
        occupiedAreas.Add(placement.Value.Bounds);
        
        PickUpInteraction interaction = new PickUpInteraction(itemPickUp);
        interaction.OnItemPickedUp += PickUpItem;
        itemPickUp.Interaction = interaction;
        InteractionSystem.RegisterInteraction(interaction);

        return itemPickUp;
    }
    
    private void PickUpItem(ItemPickUp pickable)
    {
        ItemPickupRequested?.Invoke(pickable);
    }

    private bool CreateGhost(Vector2 position)
    {
        if (ghosts.Count >= MaxGhosts)
            return false;
        
        Ghost ghost = CreateLevelCharacter<Ghost>();
        ghost.Transform.Position = position;
        ghost.Transform.Scale = new Vector2(0.6f, 0.6f);
        ghosts.Add(ghost);
        return true;
    }

    private void CreateMerchant()
    {
        merchant = CreateLevelCharacter<Merchant>();
        merchant.SetOffMapPosition(new Vector2(
            gameContext.WorldSize.X + merchant.Width,
            gameContext.WorldSize.Y + merchant.Height));
        merchant.SetOnMapPosition(new Vector2(3859, 3549));
        
        TraderInteraction interaction = new TraderInteraction(merchant);
        interaction.OnTradeRequested += ShowMarket;
        merchant.TraderInteraction = interaction;
        InteractionSystem.RegisterInteraction(interaction);
        
        merchant.Inventory = InventoryGenerator.CreateMerchantInventory(
            gameContext.RandomService, HasBlueprintTarget);
    }
    
    private void CreateProps()
    {
        CreateLevelObject(PropFactory,"crypt",  new Vector2(337, 1900));
        CreateLevelObject(PropFactory,"dirt",  new Vector2(500, 800));
        Prop spade = CreateLevelObject(PropFactory,"spade",  new Vector2(550, 800));
        spade.Transform.Scale = new Vector2(0.16f, 0.16f);
        Prop angel = CreateLevelObject(PropFactory,"angel",  new Vector2(4194, 2061));
        angel.Transform.Scale = new Vector2(0.6f, 0.6f);
        angel.SpriteEffect = SpriteEffects.FlipHorizontally;
    }
    
    private void CreateDecorations()
    {
        CreateHouse();
        CreateTrees();
        CreateLamps();
        CreateFences();
        CreateBenches();
        CreateFountain();
        CreateFlowerbeds();
    }

    private void CreateFountain()
    {
        UpgradableDecoration fountain = CreateLevelObject(UpgradableDecorationFactory, "Fountain", new Vector2(2074, 1636));
        fountain.DecorationType = DecorationType.FountainUpgrade;
        fountain.Transform.Scale = new Vector2(1f, 1f);
        fountain.Pivot = new Vector2(0.5f, 1f);
    }

    private void CreateHouse()
    {
        UpgradableDecoration house = CreateLevelObject(UpgradableDecorationFactory, "House", new Vector2(470, 3560));
        house.DecorationType = DecorationType.HouseUpgrade;
        house.Transform.Scale = new Vector2(1f, 1f);
        house.Pivot = new Vector2(0.5f, 1f);
        house.ShadowOffsetY = -60;
    }

    private void CreateTrees()
    {
        Vector2[] positions =
        {
            // House
            new Vector2(69, 3340),
            
            // Crypt
            new Vector2(86, 1925),
            new Vector2(720, 2425),
            
            // Angel
            new Vector2(4279, 1241),
            
            // Top
            new Vector2(259, 420),
            new Vector2(4362, 772),
        };
        foreach (Vector2 position in positions)
        {
            Decoration tree = CreateLevelObject(DecorationFactory,"tree", position);
            tree.DecorationType = DecorationType.Tree;
        }
    }

    private void CreateLamps()
    {
        // Entrance
        CreateLamp(new Vector2(2278, 3059), false, true);
        CreateLamp(new Vector2(1911, 3059), true);
        
        // House
        CreateLamp(new Vector2(875, 3209), false);
        
        // Merchant
        CreateLamp(new Vector2(3439, 3230), false);
        CreateLamp(new Vector2(4287, 3230), true, true);
        
        // Fountain
        CreateLamp(new Vector2(1561, 1246), false);
        CreateLamp(new Vector2(1561, 1812), false);
        CreateLamp(new Vector2(2650, 1246), true);
        CreateLamp(new Vector2(2650, 1812), true);
    }
    
    private void CreateLamp(Vector2 position, bool flip, bool IsUnlocked = false)
    {
        Decoration lamp = CreateLevelObject(LamppostFactory,"lampost", position);
        lamp.SpriteEffect = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        lamp.DecorationType = DecorationType.Lamp;
        if (IsUnlocked)
            lamp.ApplyBlueprint();
    }
    
    private void CreateFences()
    {
        // 170
        Vector2[] positions =
        {
            // House
            new Vector2(85, 3074),
            new Vector2(265, 3074),
            new Vector2(440, 3074),
            new Vector2(625, 3074),
            new Vector2(800, 3074),
            
            // Merchant
            new Vector2(3385, 3074),
            new Vector2(3565, 3074),
            new Vector2(3740, 3074),
            new Vector2(3925, 3074),
            new Vector2(4100, 3074),
            new Vector2(4280, 3074),
            new Vector2(4460, 3074),
            
            // Above House
            new Vector2(85, 1000),
            new Vector2(265, 1000),
            new Vector2(440, 1000),
            new Vector2(625, 1000),
            new Vector2(800, 1000),
            
            // Above Merchant
            new Vector2(3385, 1000),
            new Vector2(3565, 1000),
            new Vector2(3740, 1000),
            new Vector2(3925, 1000),
            new Vector2(4100, 1000),
            new Vector2(4280, 1000),
            new Vector2(4460, 1000),
        };
        foreach (Vector2 position in positions)
        {
            Decoration fence = CreateLevelObject(DecorationFactory,"fence", position);
            fence.Transform.Scale = new Vector2(0.8f, 0.8f);
            fence.DecorationType = DecorationType.Fence;
            if (gameContext.RandomService.Chance(0.15f))
                fence.ApplyBlueprint();
        }
    }
    
    private void CreateFlowerbeds()
    {
        Vector2[] positions =
        {
            // Entrance
            new Vector2(2106, 3530),
            
            // House
            new Vector2(281, 3583),
            new Vector2(641, 3583),
            
            // Merchant
            new Vector2(3438, 3725),
            new Vector2(4267, 3725),
            
            // Main
            new Vector2(1816, 2513),
            new Vector2(1816, 2019),
            new Vector2(2335, 2513),
            new Vector2(2335, 2019),
            
            // Crypt
            new Vector2(364, 1364),
            new Vector2(625, 2033),
            new Vector2(357, 2551),
            
            // Angel
            new Vector2(3947, 2154),
            new Vector2(4322, 2356),
            new Vector2(3666, 1228),
        };
        
        foreach (Vector2 position in positions)
        {
            Decoration flowerbed = CreateLevelObject(DecorationFactory,"flowerbed1", position);
            flowerbed.Transform.Scale = new Vector2(0.6f, 0.6f);
            flowerbed.DecorationType = DecorationType.FlowerBed;
            if (gameContext.RandomService.Chance(0.05f))
                flowerbed.ApplyBlueprint();
        }
    }

    private void CreateBenches()
    {
        // Entrance
        CreateBench(new Vector2(1606, 3154), true);
        CreateBench(new Vector2(2615, 3154));
        
        // Merchant
        CreateBench(new Vector2(3875, 3292));
        
        // Crypy
        CreateBench(new Vector2(1020, 2002));
        
        // Angel
        CreateBench(new Vector2(3570, 2161), true);
        
        // Fountain
        CreateBench(new Vector2(2082, 1037));
    }

    private void CreateBench(Vector2 position, bool isUnlocked = false)
    {
        Decoration bench = CreateLevelObject(DecorationFactory,"bench", position);
        bench.Transform.Scale = new Vector2(1f, 1f);
        bench.DecorationType = DecorationType.Bench;
        if (isUnlocked)
            bench.ApplyBlueprint();
    }
    
    private void CreateGraveSites()
    {
        // Above crypt
        CreateOccupiedGraveSite("tombstone5", new Vector2(200, 1500));
        CreateOccupiedGraveSite("tombstone1", new Vector2(550, 1500));
        CreateOccupiedGraveSite("tombstone2", new Vector2(900, 1500));
        CreateLockedGraveSite(new Vector2(1250, 1500));    
        
        // Above house
        CreateOccupiedGraveSite("tombstone2", new Vector2(200, 2800));
        CreateOccupiedGraveSite("tombstone6", new Vector2(550, 2800));
        CreateOccupiedGraveSite("tombstone1", new Vector2(900, 2800));
        
        // Above Merchant
        CreateOccupiedGraveSite("tombstone1", new Vector2(3400, 2800));
        CreateOccupiedGraveSite("tombstone4", new Vector2(3700, 2800));
        CreateOccupiedGraveSite("tombstone5", new Vector2(4000, 2800));
        CreateOccupiedGraveSite("tombstone7", new Vector2(4300, 2800));
        
        // Above Angel
        CreateOccupiedGraveSite("tombstone8", new Vector2(3400, 1655));
        CreateOccupiedGraveSite("tombstone1", new Vector2(3700, 1655));
        CreateOccupiedGraveSite("tombstone3", new Vector2(4000, 1655));
        CreateOccupiedGraveSite("tombstone2", new Vector2(4300, 1655));
        
        // Top
        CreateOccupiedGraveSite("tombstone6", new Vector2(1578, 755));
        CreateOccupiedGraveSite("tombstone3", new Vector2(2646, 755));
        
        CreateLockedGraveSite(new Vector2(4098, 400)); 
        CreateLockedGraveSite(new Vector2(3598, 400)); 
        CreateLockedGraveSite(new Vector2(3098, 400)); 
        CreateLockedGraveSite(new Vector2(2598, 400)); 
        CreateLockedGraveSite(new Vector2(2098, 400));   
        CreateLockedGraveSite(new Vector2(1598, 400));   
        CreateLockedGraveSite(new Vector2(1098, 400));   
        CreateLockedGraveSite(new Vector2(598, 400));   
    }
    
    private void CreateLockedGraveSite(Vector2 position)
    {
        GraveSite graveSite = CreateGraveSite(GraveSiteStatus.Locked, "sign", "grave_locked", position);
    }
    
    private void CreateOccupiedGraveSite(string name, Vector2 position)
    {
        GraveState randomState = GraveSiteGenerator.GetRandomState(gameContext.RandomService);

        GraveSite graveSite = CreateGraveSite(GraveSiteStatus.Occupied,name, name, position, randomState);

        GraveSiteData data = GraveSiteGenerator.Generate(gameContext.RandomService);
        graveSite.Tombstone.SetData(data);
    }
    
    private GraveSite CreateGraveSite(GraveSiteStatus status,
        string tombstoneName, string graveName, Vector2 position,
        GraveState? state = null)
    {
        Tombstone tombstone = CreateLevelObject(TombstoneFactory, tombstoneName, position);
        Prop grave = CreateLevelObject(PropFactory, graveName, position);
        Prop dirt = CreateLevelObject(PropFactory, "dirt", position);

        GraveSite graveSite = state.HasValue
            ? new GraveSite(status, state.Value)
            : new GraveSite(status);

        graveSite.Transform.Position = position;
        graveSite.SetTombstone(tombstone);
        graveSite.SetGrave(grave);
        graveSite.SetDirt(dirt);

        RegisterGraveSiteInteraction(tombstone);

        graveSites.Add(graveSite);

        return graveSite;
    }
    
    private void RegisterGraveSiteInteraction(Tombstone tombstone)
    {
        TombstoneInteraction interaction = new TombstoneInteraction(tombstone);

        interaction.OnTombstoneRead += InteractWithGravesite;
        tombstone.Interaction = interaction;
        InteractionSystem.RegisterInteraction(interaction);
    }

    private void InteractWithGravesite(Tombstone tombstone)
    {
        InteractionSystem.ClearState();
        GraveInteractionRequested?.Invoke(tombstone.ParentSite);
    }

    private Prop PropFactory(string spriteName)
    {
        return new Prop(spriteName);
    }

    private Decoration DecorationFactory(string spriteName)
    {
        return new Decoration(spriteName);
    }
    
    private Decoration LamppostFactory(string spriteName)
    {
        return new Lamppost(spriteName);
    }
    
    private UpgradableDecoration UpgradableDecorationFactory(string spriteName)
    {
        return new UpgradableDecoration(spriteName);
    }

    private Tombstone TombstoneFactory(string spriteName)
    {
        return new Tombstone(spriteName);
    }

    private ItemPickUp LootFactory(string spriteName)
    {
        return new ItemPickUp(spriteName);
    }
    
    private void LoadGroundTextures()
    {
        for (int i = 0; i < MapTileTypesCount; i++)
        {
            SpriteManager.AddSprite($"ground{i}", $"Images/Environment_New/earth_tile{i}");
        }
        SpriteManager.AddSprite($"road", $"Images/Environment_New/road");
        SpriteManager.AddSprite($"road1", $"Images/Environment_New/road1");
        SpriteManager.AddSprite($"road2", $"Images/Environment_New/road2");
        SpriteManager.AddSprite($"road3", $"Images/Environment_New/road3");
    }

    private void LoadPropTextures()
    {
        SpriteManager.AddSprite("crypt", "Images/Props/Crypt");
        SpriteManager.AddSprite("angel", "Images/Props/Angel");
        SpriteManager.AddSprite("dirt", "Images/Props/Dirt");
        SpriteManager.AddSprite("spade", "Images/Props/Spade");
    }
    
    private void LoadDecorationTextures()
    {
        SpriteManager.AddSprite("tree", "Images/Props/Tree");
        SpriteManager.AddSprite("bench", "Images/Props/Bench");
        SpriteManager.AddSprite("flowerbed1", "Images/Props/Flowerbed1");
        SpriteManager.AddSprite("flowerbed2", "Images/Props/Flowerbed2");
        SpriteManager.AddSprite("fence", "Images/Props/Fence");
        SpriteManager.AddSprite("lampost", "Images/Props/Lampost");
        SpriteManager.AddSprite("House1", "Images/Props/House1");
        SpriteManager.AddSprite("House2", "Images/Props/House2");
        SpriteManager.AddSprite("House3", "Images/Props/House3");
        SpriteManager.AddSprite("Fountain1", "Images/Props/Fountain1");
        SpriteManager.AddSprite("Fountain2", "Images/Props/Fountain2");
        SpriteManager.AddSprite("Fountain3", "Images/Props/Fountain3");
        SpriteManager.AddSprite("DecorPlaceholder", "Images/Props/DecorPlaceholder");
    }
    
    private void LoadTombstoneTextures()
    {
        SpriteManager.AddSprite("tombstone1", "Images/Props/Tombstone1");
        SpriteManager.AddSprite("tombstone2", "Images/Props/Tombstone2");
        SpriteManager.AddSprite("tombstone3", "Images/Props/Tombstone3");
        SpriteManager.AddSprite("tombstone4", "Images/Props/Tombstone4");
        SpriteManager.AddSprite("tombstone5", "Images/Props/Tombstone5");
        SpriteManager.AddSprite("tombstone6", "Images/Props/Tombstone6");
        SpriteManager.AddSprite("tombstone7", "Images/Props/Tombstone7");
        SpriteManager.AddSprite("tombstone8", "Images/Props/Tombstone8");
        SpriteManager.AddSprite("tombstone9", "Images/Props/Tombstone9");
        SpriteManager.AddSprite("sign", "Images/Props/Sign");

        SpriteManager.AddSprite("grave_earth", "Images/Environment_New/grave_earth");
        SpriteManager.AddSprite("grave_digged", "Images/Environment_New/grave_digged");
        SpriteManager.AddSprite("grave_broken", "Images/Environment_New/grave_broken");
        SpriteManager.AddSprite("grave_locked", "Images/Environment_New/grave_locked");
        SpriteManager.AddSprite("grave_prepared", "Images/Environment_New/grave_prepared");
    }
    
    private void LoadLootTextures()
    {
        for (int i = 1; i < 49; i++)
        {
            if (i == 40)
                continue;
            
            SpriteManager.AddSprite($"Icon{i}",  $"Images/Loot/Icon{i}");
        }
    }
    
    private void LoadFoodTextures()
    {
        for (int i = 1; i < 11; i++)
        {
            SpriteManager.AddSprite($"Food{i}",  $"Images/Food/Food{i}");
        }
    }
    
    private void LoadDecorationIconsTextures()
    {
        SpriteManager.AddSprite("treeIcon", "Images/Props/TreeIcon");
        SpriteManager.AddSprite("benchIcon", "Images/Props/BenchIcon");
        SpriteManager.AddSprite("flowerbedIcon", "Images/Props/FlowerbedIcon");
        SpriteManager.AddSprite("fenceIcon", "Images/Props/FenceIcon");
        SpriteManager.AddSprite("lampostIcon", "Images/Props/LampostIcon");
        SpriteManager.AddSprite("houseIcon", "Images/Props/HouseIcon");
        SpriteManager.AddSprite("fountainIcon", "Images/Props/FountainIcon");
    }
    
    private void LoadCharacterTextures()
    {
        SpriteManager.AddSprite("digger", "Images/Characters/digger", columns: 4, rows: 4);
        SpriteManager.AddSprite("merchant", "Images/Characters/merchant", columns: 4, rows: 4);
        SpriteManager.AddSprite("ghost", "Images/Characters/ghost", columns: 4, rows: 4);
    }
    
    private void ShowMarket()
    {
        InteractionSystem.ClearState();
        merchant.ChangeState(MerchantState.Trading);
        MarketOpenRequested?.Invoke(merchant);
    }

    public void MarketClosed()
    {
        merchant.ChangeState(MerchantState.Idle);
    }
    
    public bool HasPreparedGraveSites()
    {
        return graveSites.Any(graveSite => graveSite.Status == GraveSiteStatus.Prepared);
    }
    
    public void OccupyPreparedGraveSite()
    {
        GraveSite preparedGraveSite = GetPreparedGraveSite();
        if (preparedGraveSite == null)
            return;

        string tombstoneName = GraveSiteGenerator.GetRandomTombstoneSprite(gameContext.RandomService);
        GraveSiteData data = GraveSiteGenerator.Generate(gameContext.RandomService);
        if (!preparedGraveSite.Occupy(data, tombstoneName))
            return;
        
        GraveOccupied?.Invoke(preparedGraveSite);
    }

    private bool HasBlueprintTarget(DecorationType decorationType)
    {
        return decorations.Any(decoration =>
            decoration.DecorationType == decorationType &&
            decoration.CanApplyBlueprint);
    }
    
    private GraveSite GetPreparedGraveSite()
    {
        return graveSites.FirstOrDefault(graveSite => graveSite.Status == GraveSiteStatus.Prepared);
    }
}