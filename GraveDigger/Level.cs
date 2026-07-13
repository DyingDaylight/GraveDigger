using System;
using System.Collections.Generic;
using System.Linq;
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
        List<Rectangle> occupiedAreas = props.Select(prop => prop.GetDestRectangle(prop.sourceRectangle)).ToList();

        foreach (ItemData item in loot)
        {
            ItemPickUp itemPickUp =CreatePickupItem(tombstone, item, occupiedAreas);
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
        RegisterObject(prop);
        props.Add(prop);
        return prop;
    }

    private ItemPickUp CreatePickupItem(Prop sourceProp, ItemData item, List<Rectangle> occupiedAreas)
    {
        ItemPickUp itemPickUp = CreateLevelObject(LootFactory, item.SpriteName, sourceProp.Transform.Position);
        itemPickUp.Transform.Scale = new Vector2(2.5f, 2.5f);
        itemPickUp.SetData(item);
        
        Vector2 origin = new Vector2(sourceProp.Left + sourceProp.Width * 0.5f, sourceProp.Bottom);

        Point itemSize = new(
            itemPickUp.destRectangle.Width,
            itemPickUp.destRectangle.Height
        );

        Vector2? position = LootPlacementService.FindFreePosition(origin, itemSize, occupiedAreas);

        if (position.HasValue)
        {
            itemPickUp.Transform.Position = position.Value;
            occupiedAreas.Add(itemPickUp.GetDestRectangle(itemPickUp.sourceRectangle));
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
    
    private void CreateProps()
    {
        CreateLevelObject(PropFactory,"crypt",  new Vector2(1300, 350));
        CreateLevelObject(PropFactory,"tree",  new Vector2(1600, 700));
        CreateLevelObject(PropFactory,"tree",  new Vector2(800, 800));
        CreateLevelObject(PropFactory,"dirt",  new Vector2(1300, 800));
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
        CreateTombstone("tombstone5", new Vector2(100, 350));
        CreateTombstone("tombstone1", new Vector2(300, 350));
        CreateTombstone("tombstone2", new Vector2(500, 350));
        CreateTombstone("tombstone3", new Vector2(700, 350));
        
        CreateTombstone("tombstone1", new Vector2(100, 250));
        CreateTombstone("tombstone4", new Vector2(300, 250));
        CreateTombstone("tombstone5", new Vector2(500, 250));
        CreateTombstone("tombstone6", new Vector2(700, 250));
        
        CreateTombstone("tombstone2", new Vector2(100, 150));
        CreateTombstone("tombstone6", new Vector2(300, 150));
        CreateTombstone("tombstone1", new Vector2(500, 150));
        CreateTombstone("tombstone3", new Vector2(700, 150));
    }

    private void CreateTombstone(string name, Vector2 position)
    {
        TombstoneData tombstoneData = TombstoneGenerator.GenerateTombstoneData(gameContext.RandomService);
        
        Tombstone tombstone = CreateLevelObject(TombstoneFactory, name, position);
        tombstone.Transform.Scale = new Vector2(0.8f, 0.8f);
        tombstone.SetData(tombstoneData);
        tombstone.State = gameContext.RandomService.RandomEnum<TombstoneState>();
        
        TombstoneInteraction interaction = new TombstoneInteraction(tombstone);
        interaction.OnTombstoneRead += OpenTombstone;
        tombstone.Interaction = interaction;
        InteractionSystem.RegisterInteraction(interaction);
    }
    
    private void OpenTombstone(Tombstone tombstone)
    {
        InteractionSystem.ClearState();
        gameplayActions.OpenTombstone(tombstone);
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
        SpriteManager.AddSprite("tree", "Images/Props/Tree");
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