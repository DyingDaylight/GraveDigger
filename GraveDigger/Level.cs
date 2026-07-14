using System;
using System.Collections.Generic;
using System.Linq;
using GraveDigger.Core;
using GraveDigger.Data;
using GraveDigger.Interactions;
using GraveDigger.Items;
using GraveDigger.Props;
using GraveDigger.Utils;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GraveDigger;

public class Level : IUpdatable, IDrawable
{
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

    private readonly GameContext gameContext;
    private readonly IGameplayActions gameplayActions;
    
    public InteractionSystem InteractionSystem { get; private set; }

    public Level(GameContext gameContext, IGameplayActions gameplayActions)
    {
        this.gameContext = gameContext;
        this.gameplayActions = gameplayActions;
        
        InteractionSystem = new InteractionSystem(gameContext.CoordinatesConverter);
    }
    
    public void LoadTextures()
    {
        LoadGroundTextures();
        LoadPropTextures();
        LoadTombstoneTextures();
        LoadLootTextures();
    }
    
    public void Start()
    { 
        CreateMap();
        CreateProps();
        CreateLamps();
        CreateTombstones();
        
        foreach (IUpdatable updatable in updatables)
            updatable.Start();
        
        gameplayActions.OnGraveDug += SpawnGraveDirt;
        gameplayActions.OnGraveRepaired += RemoveGraveDirt;
    }

    public void Update(GameTime gameTime)
    {
        map.Update(gameTime);
        foreach (IUpdatable updatable in updatables)
            updatable.Update(gameTime);
        
        foreach (Collider collider in colliders)
            collider.Update(gameTime);
        
        InteractionSystem.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        map.Draw(spriteBatch);
        foreach (IDrawable drawable in drawables)
            drawable.Draw(spriteBatch);
        
        foreach (Collider collider in colliders)
            collider.Draw(spriteBatch);
    }
    
    public void SpawnLoot(List<ItemData> loot, Tombstone tombstone)
    {
        List<Rectangle> occupiedAreas = props.Select(prop => prop.GetDestRectangle(prop.SourceRectangle)).ToList();

        foreach (ItemData item in loot)
        {
            ItemPickUp itemPickUp = CreatePickupItem(tombstone, item, occupiedAreas);
            itemPickUp.Start();
        }
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
    }
    
    private void CreateMap()
    {
        map.TileMap = tileMapSchema;
        map.Start();
    }    
    
    private T CreateLevelObject<T>(Func<string, T> factory, string name, Vector2 position) where T : Prop
    {
        T prop = factory(name);
        prop.Transform.Position = position;
        prop.CastSHadow = true;
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
        gameplayActions.PickupItem(pickable.ItemData);
        RemovePickup(pickable);
    }
    
    private void RemovePickup(ItemPickUp pickable)
    {
        InteractionSystem.UnregisterInteraction(pickable.Interaction);
        UnregisterObject(pickable);
        props.Remove(pickable);
    }
    
    private void SpawnGraveDirt(Tombstone tombstone)
    {
        // TODO: move dirt to Tombstone
        // TODO: change sprites in Tombstone depending on state
        tombstone.GraveTile?.ChangeSprite("grave_digged");

        var graveTile = tombstone.GraveTile;
        if (graveTile == null)
            return;
        
        Vector2 dirtPosition = new Vector2(graveTile.Transform.Position.X, graveTile.Transform.Position.Y + 140);
        CreateLevelObject(PropFactory, "dirt", dirtPosition);
    }

    private void RemoveGraveDirt(Tombstone tombstone)
    {
        tombstone.GraveTile?.ChangeSprite("grave_earth");

        var graveTile = tombstone.GraveTile;
        if (graveTile == null)
            return;
  
        Vector2 dirtPosition = new Vector2(graveTile.Transform.Position.X, graveTile.Transform.Position.Y + 140);
        Prop dirtToRemove = props.FirstOrDefault(p => p.SourceRectangle != null && p.Transform.Position == dirtPosition);
    
        if (dirtToRemove != null)
        {
            UnregisterObject(dirtToRemove);
            props.Remove(dirtToRemove);
        }
    }
    
    private void CreateProps()
    {
        CreateLevelObject(PropFactory,"crypt",  new Vector2(1300, 350));
        CreateLevelObject(PropFactory,"tree",  new Vector2(1600, 700));
        CreateLevelObject(PropFactory,"tree",  new Vector2(800, 800));
        CreateLevelObject(PropFactory,"dirt",  new Vector2(500, 800));
        CreateLevelObject(PropFactory,"spade",  new Vector2(1300, 820));
        CreateLevelObject(PropFactory,"angel",  new Vector2(1600, 250));
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
        Prop lamp = CreateLevelObject(PropFactory,"lampost", position);
        lamp.SpriteEffect = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
    }
    
    private void CreateTombstones()
    {
        CreateTombstone("tombstone5", new Vector2(200, 1500));
        CreateTombstone("tombstone1", new Vector2(550, 1500));
        CreateTombstone("tombstone2", new Vector2(900, 1500));
        
        CreateTombstone("tombstone1", new Vector2(200, 600));
        CreateTombstone("tombstone4", new Vector2(550, 600));
        CreateTombstone("tombstone5", new Vector2(900, 600));
        
        CreateTombstone("tombstone6", new Vector2(200, 1000));
        CreateTombstone("tombstone3", new Vector2(550, 1000));
        
        CreateTombstone("tombstone2", new Vector2(200, 350));
        CreateTombstone("tombstone6", new Vector2(550, 350));
        CreateTombstone("tombstone1", new Vector2(900, 350));
    }

    private void CreateTombstone(string name, Vector2 position)
    {
        GraveSiteData graveSiteData = GraveSiteGenerator.Generate(gameContext.RandomService);
    
        Prop earth = CreateLevelObject(PropFactory, "grave_earth", position);
    
        Vector2 tombstonePosition = new Vector2(position.X, position.Y - 190);
        Tombstone tomb = CreateLevelObject(TombstoneFactory, name, tombstonePosition);
        tomb.Transform.Scale = new Vector2(0.3f, 0.3f);
        tomb.SetData(graveSiteData);
        tomb.State = gameContext.RandomService.RandomEnum<GraveSiteState>();
    
        tomb.GraveTile = earth;
        tomb.GraveTile.Mode = SortingMode.Fixed;
        tomb.GraveTile.CastSHadow = false;
    
        TombstoneInteraction interaction = new TombstoneInteraction(tomb);
        interaction.OnTombstoneRead += OpenTombstone;
        tomb.Interaction = interaction;
        InteractionSystem.RegisterInteraction(interaction);
    }

    private void OpenTombstone(Tombstone obj)
    {
        InteractionSystem.ClearState();
        gameplayActions.OpenTombstone(obj);
    }

    private Prop PropFactory(string spriteName)
    {
        return new Prop(spriteName);
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
    }

    private void LoadPropTextures()
    {
        SpriteManager.AddSprite("tree", "Images/Props/Tombstone7");
        SpriteManager.AddSprite("crypt", "Images/Props/Crypt");
        SpriteManager.AddSprite("angel", "Images/Props/Angel");
        SpriteManager.AddSprite("dirt", "Images/Props/Dirt");
        SpriteManager.AddSprite("spade", "Images/Props/Spade");
        SpriteManager.AddSprite("lampost", "Images/Props/Lampost");
    }
    
    private void LoadTombstoneTextures()
    {
        SpriteManager.AddSprite("tombstone1", "Images/Props/Tombstone1");
        SpriteManager.AddSprite("tombstone2", "Images/Props/Tombstone2");
        SpriteManager.AddSprite("tombstone3", "Images/Props/Tombstone3");
        SpriteManager.AddSprite("tombstone4", "Images/Props/Tombstone4");
        SpriteManager.AddSprite("tombstone5", "Images/Props/Tombstone5");
        SpriteManager.AddSprite("tombstone6", "Images/Props/Tombstone6");

        SpriteManager.AddSprite("grave_earth", "Images/Environment_New/grave_earth");
        SpriteManager.AddSprite("grave_digged", "Images/Environment_New/grave_digged");
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
}