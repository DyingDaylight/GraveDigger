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
    private const int MaxGhosts = 10;
    private const int HungerPerDay = 10;
    private const int MapTileTypesCount = 3;
    private readonly int[,] tileMapSchema =
    {
        { 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2 },
        { 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2 },
        { 1, 2, 2, 2, 0, 0, 2, 2, 0, 0, 0, 0, 0, 1, 1 },
        { 1, 2, 2, 2, 0, 2, 2, 1, 0, 0, 0, 0, 0, 1, 1 },
        { 1, 2, 2, 2, 2, 2, 2, 1, 1, 2, 1, 2, 2, 2, 2 },
        { 1, 2, 0, 2, 1, 2, 2, 1, 0, 0, 0, 1, 0, 0, 0 },
        { 0, 2, 2, 2, 0, 2, 2, 1, 0, 0, 0, 0, 1, 0, 0 },
        { 0, 1, 2, 2, 2, 2, 2, 1, 0, 0, 0, 2, 0, 1, 1 },
        { 0, 2, 2, 2, 0, 2, 2, 1, 1, 1, 1, 0, 0, 0, 2 },
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

    public event Action ReputationRecalculationRequested;
    public event Action<GraveSite> GraveOpenRequested;
    public event Action<Merchant> MarketOpenRequested;
    public event Action<ItemPickUp> ItemPickupRequested;
    
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
        CreateTombstones();
        CreateDecorations();
        
        CreateMerchant();
        
        Player = CreateLevelCharacter<Player>();
        Player.Transform.Position = new Vector2(gameContext.ScreenSize.X * 0.5f, 
            gameContext.ScreenSize.Y * 0.5f);
        Player.SetWorldSize(gameContext.WorldSize);
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
        foreach (Prop prop in props)
        {
            if (prop is IDailyUpdatable dailyUpdatable)
                dailyUpdatable.AdvanceDay(day);
        }
        
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
            itemPickUp.Start();
        }
    }
    
    public void SpawnUndead(EnemyType enemyType, GraveSite graveSite)
    {
        switch (enemyType)
        {
            case EnemyType.Ghost:
                CreateGhost(graveSite.Transform.Position);
                AudioManager.Instance.PlaySFX("ghost-spawn");
                ReputationRecalculationRequested?.Invoke();
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

        Vector2? position = LootPlacementService.FindFreePosition(origin, itemSize, occupiedAreas);

        if (position.HasValue)
        {
            itemPickUp.Transform.Position = position.Value;
            occupiedAreas.Add(itemPickUp.GetDestRectangle(itemPickUp.SourceRectangle));
        }
        
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

    private void CreateGhost(Vector2 position)
    {
        if (ghosts.Count >= MaxGhosts)
            return;
        
        Ghost ghost = CreateLevelCharacter<Ghost>();
        ghost.Transform.Position = position;
        ghost.Transform.Scale = new Vector2(0.6f, 0.6f);
        ghosts.Add(ghost);
    }

    private void CreateMerchant()
    {
        merchant = CreateLevelCharacter<Merchant>();
        merchant.SetOffMapPosition(new Vector2(
            gameContext.WorldSize.X + merchant.Width,
            gameContext.WorldSize.Y + merchant.Height));
        merchant.SetOnMapPosition(new Vector2(1920, 1080));
        
        TraderInteraction interaction = new TraderInteraction(merchant);
        interaction.OnTradeRequested += ShowMarket;
        merchant.TraderInteraction = interaction;
        InteractionSystem.RegisterInteraction(interaction);
        
        merchant.Inventory = InventoryGenerator.CreateMerchantInventory(
            gameContext.RandomService, HasBlueprintTarget);
    }
    
    private void CreateProps()
    {
        CreateLevelObject(PropFactory,"crypt",  new Vector2(1300, 350));
        CreateLevelObject(PropFactory,"dirt",  new Vector2(500, 800));
        CreateLevelObject(PropFactory,"spade",  new Vector2(1300, 820));
        CreateLevelObject(PropFactory,"angel",  new Vector2(1600, 250));
    }
    
    private void CreateDecorations()
    {
        CreateHouse();
        CreateTrees();
        CreateLamps();
        CreateFences();
        CreateBenches();
        CreateFlowerbeds();
    }

    private void CreateHouse()
    {
        DiggerHouse house = CreateLevelObject(HouseFactory, "", new Vector2(1260, 1080));
        house.DecorationType = DecorationType.HouseUpgrade;
        house.Transform.Scale = new Vector2(1f, 1f);
        house.Pivot = new Vector2(0.5f, 1f);
        house.ShadowOffsetY = -60;
    }

    private void CreateTrees()
    {
        Vector2[] positions =
        {
            new Vector2(1600, 700),
            new Vector2(800, 800),
            new Vector2(1500, 1700),
        };
        foreach (Vector2 position in positions)
        {
            Decoration tree = CreateLevelObject(DecorationFactory,"tree", position);
            tree.DecorationType = DecorationType.Tree;
        }
    }

    private void CreateLamps()
    {
        CreateLamp(new Vector2(890, 130), false);
        CreateLamp(new Vector2(1030, 380), true);
        CreateLamp(new Vector2(890, 690), false);
        CreateLamp(new Vector2(1030, 940), true);
    }
    
    private void CreateLamp(Vector2 position, bool flip)
    {
        Decoration lamp = CreateLevelObject(LamppostFactory,"lampost", position);
        lamp.SpriteEffect = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        lamp.DecorationType = DecorationType.Lamp;
    }
    
    private void CreateFences()
    {
        Vector2[] positions =
        {
            new Vector2(1500, 1600),
            new Vector2(1500, 1500),
            new Vector2(1500, 1700),
        };
        foreach (Vector2 position in positions)
        {
            Decoration fence = CreateLevelObject(DecorationFactory,"fence", position);
            fence.Transform.Scale = new Vector2(0.8f, 0.8f);
            fence.DecorationType = DecorationType.Fence;
        }
    }
    
    private void CreateFlowerbeds()
    {
        Vector2[] positions =
        {
            new Vector2(1700, 1600),
            new Vector2(1700, 1500),
            new Vector2(1700, 1700),
        };
        foreach (Vector2 position in positions)
        {
            Decoration flowerbed = CreateLevelObject(DecorationFactory,"flowerbed1", position);
            flowerbed.Transform.Scale = new Vector2(0.6f, 0.6f);
            flowerbed.DecorationType = DecorationType.FlowerBed;
        }
    }

    private void CreateBenches()
    {
        CreateBench(new Vector2(2000, 500));
        CreateBench(new Vector2(1300, 350));
        CreateBench(new Vector2(2400, 1000));
    }

    private void CreateBench(Vector2 position)
    {
        Decoration bench = CreateLevelObject(DecorationFactory,"bench", position);
        bench.Transform.Scale = new Vector2(1f, 1f);
        bench.DecorationType = DecorationType.Bench;
    }
    
    private void CreateTombstones()
    {
        CreateGraveSite("tombstone5", new Vector2(200, 1500));
        CreateGraveSite("tombstone1", new Vector2(550, 1500));
        CreateGraveSite("tombstone2", new Vector2(900, 1500));
    
        CreateGraveSite("tombstone1", new Vector2(200, 600));
        CreateGraveSite("tombstone4", new Vector2(550, 600));
        CreateGraveSite("tombstone5", new Vector2(900, 600));
    
        CreateGraveSite("tombstone6", new Vector2(200, 1000));
        CreateGraveSite("tombstone3", new Vector2(550, 1000));
    
        CreateGraveSite("tombstone2", new Vector2(200, 350));
        CreateGraveSite("tombstone6", new Vector2(550, 350));
        CreateGraveSite("tombstone1", new Vector2(900, 350));
    }
    
    private void CreateGraveSite(string name, Vector2 position)
    {
        GraveSiteData graveSiteData = GraveSiteGenerator.Generate(gameContext.RandomService);
        GraveSiteState randomState = gameContext.RandomService.RandomEnum<GraveSiteState>();

        Tombstone tombstone = CreateLevelObject(TombstoneFactory, name, position);
        
        GraveTile graveTile = CreateLevelObject(GraveFactory, name, position);
        graveTile.DecayInterval = gameContext.RandomService.Next(2, 5);
        graveTile.State = randomState;

        Prop dirt = CreateLevelObject(PropFactory, "dirt", position);
        
        GraveSite graveSite = new GraveSite();
        graveSite.Transform.Position = position;
        graveSite.SetTombstone(tombstone);
        graveSite.SetGrave(graveTile);
        graveSite.SetDirt(dirt);
        
        graveSite.Tombstone.SetData(graveSiteData);
        
        TombstoneInteraction interaction = new TombstoneInteraction(tombstone);
        interaction.OnTombstoneRead += OpenTombstone;
        tombstone.Interaction = interaction;
        InteractionSystem.RegisterInteraction(interaction);

        graveSites.Add(graveSite);
    }

    private void OpenTombstone(Tombstone tombstone)
    {
        InteractionSystem.ClearState();
        GraveOpenRequested?.Invoke(tombstone.ParentSite);
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
    
    private DiggerHouse HouseFactory(string spriteName)
    {
        return new DiggerHouse();
    }

    private Tombstone TombstoneFactory(string spriteName)
    {
        return new Tombstone(spriteName);
    }
    
    private GraveTile GraveFactory(string spriteName)
    {
        return new GraveTile(spriteName);
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
        SpriteManager.AddSprite("tombstone9", "Images/Props/Tombstone9");

        SpriteManager.AddSprite("grave_earth", "Images/Environment_New/grave_earth");
        SpriteManager.AddSprite("grave_digged", "Images/Environment_New/grave_digged");
        SpriteManager.AddSprite("grave_broken", "Images/Environment_New/grave_broken");
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

    private bool HasBlueprintTarget(DecorationType decorationType)
    {
        return decorations.Any(decoration =>
            decoration.DecorationType == decorationType &&
            decoration.CanApplyBlueprint);
    }
}