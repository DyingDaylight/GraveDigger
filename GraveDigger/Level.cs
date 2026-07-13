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

public class Level
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
    
    private List<Prop> props = new List<Prop>();
    private Map map = new Map();
    
    private List<IUpdatable> updatables = new();
    private List<IDrawable> drawables = new();
    private List<Collider> colliders = new();

    public InteractionSystem InteractionSystem { get; private set; }

    private GameContext gameContext;
    private IGameplayActions gameplayActions;

    public Level(GameContext gameContext, IGameplayActions gameplayActions)
    {
        this.gameContext = gameContext;
        this.gameplayActions = gameplayActions;
    }
    
    public void Start()
    {
        InteractionSystem = new InteractionSystem(gameContext.CoordinatesConverter);
        
        gameplayActions.OnGraveDug += SpawnGraveDirt;
        gameplayActions.OnGraveRepaired += RemoveGraveDirt;
        /*if (gameplayActions is GameplayCoordinator coordinator)
        {
            coordinator.OnGraveDug += SpawnGraveDirt;
            coordinator.OnGraveRepaired += RemoveGraveDirt;
        }*/
        
        CreateMap();
        CreateProps();
        CreateLamps();
        CreateTombstones();
    }

    public void Update(GameTime gameTime)
    {
        map.Update(gameTime);
        foreach (IUpdatable updatable in updatables)
            updatable.Update(gameTime);
        
        foreach (Collider collider in colliders)
            collider.Update(gameTime);
        
        InteractionSystem.Update(gameTime);
        
        //foreach (Interaction interaction in interactions)
        //    interaction.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        map.Draw(spriteBatch);
        foreach (IDrawable drawable in drawables)
            drawable.Draw(spriteBatch);
        
        foreach (Collider collider in colliders)
            collider.Draw(spriteBatch);
    }
    
    public void LoadTextures()
    {
        /*for (int i = 0; i < MapTilesAmount; i++)
        {
            SpriteManager.AddSprite($"ground{i}", $"Images/Ground/ground{i}");
        }*/
        for (int i = 0; i < MapTileTypesCount; i++)
        {
            SpriteManager.AddSprite($"ground{i}", $"Images/Environment_New/earth_tile{i}");
        }
        SpriteManager.AddSprite("tree", "Images/Props/Tombstone7");
        SpriteManager.AddSprite("crypt", "Images/Props/Crypt");
        SpriteManager.AddSprite("angel", "Images/Props/Angel");
        SpriteManager.AddSprite("dirt", "Images/Props/Dirt");
        SpriteManager.AddSprite("spade", "Images/Props/Spade");
        SpriteManager.AddSprite("lampost", "Images/Props/Lampost");
        SpriteManager.AddSprite("tombstone1", "Images/Props/Tombstone1");
        SpriteManager.AddSprite("tombstone2", "Images/Props/Tombstone2");
        SpriteManager.AddSprite("tombstone3", "Images/Props/Tombstone3");
        SpriteManager.AddSprite("tombstone4", "Images/Props/Tombstone4");
        SpriteManager.AddSprite("tombstone5", "Images/Props/Tombstone5");
        SpriteManager.AddSprite("tombstone6", "Images/Props/Tombstone6");

        SpriteManager.AddSprite("grave_earth", "Images/Environment_New/grave_earth");
        SpriteManager.AddSprite("grave_digged", "Images/Environment_New/grave_digged");
        
        SpriteManager.AddSprite("Icon1",  "Images/Loot/Icon1");
        SpriteManager.AddSprite("Icon2",  "Images/Loot/Icon2");
        SpriteManager.AddSprite("Icon3",  "Images/Loot/Icon3");
        SpriteManager.AddSprite("Icon4",  "Images/Loot/Icon4");
        SpriteManager.AddSprite("Icon5",  "Images/Loot/Icon5");
        SpriteManager.AddSprite("Icon6",  "Images/Loot/Icon6");
        SpriteManager.AddSprite("Icon7",  "Images/Loot/Icon7");
        SpriteManager.AddSprite("Icon8",  "Images/Loot/Icon8");
        SpriteManager.AddSprite("Icon9",  "Images/Loot/Icon9");
        SpriteManager.AddSprite("Icon10", "Images/Loot/Icon10");
        SpriteManager.AddSprite("Icon11", "Images/Loot/Icon11");
        SpriteManager.AddSprite("Icon12", "Images/Loot/Icon12");
        SpriteManager.AddSprite("Icon13", "Images/Loot/Icon13");
        SpriteManager.AddSprite("Icon14", "Images/Loot/Icon14");
        SpriteManager.AddSprite("Icon15", "Images/Loot/Icon15");
        SpriteManager.AddSprite("Icon16", "Images/Loot/Icon16");
        SpriteManager.AddSprite("Icon17", "Images/Loot/Icon17");
        SpriteManager.AddSprite("Icon18", "Images/Loot/Icon18");
        SpriteManager.AddSprite("Icon19", "Images/Loot/Icon19");
        SpriteManager.AddSprite("Icon20", "Images/Loot/Icon20");
        SpriteManager.AddSprite("Icon21", "Images/Loot/Icon21");
        SpriteManager.AddSprite("Icon22", "Images/Loot/Icon22");
        SpriteManager.AddSprite("Icon23", "Images/Loot/Icon23");
        SpriteManager.AddSprite("Icon24", "Images/Loot/Icon24");
        SpriteManager.AddSprite("Icon25", "Images/Loot/Icon25");
        SpriteManager.AddSprite("Icon26", "Images/Loot/Icon26");
        SpriteManager.AddSprite("Icon27", "Images/Loot/Icon27");
        SpriteManager.AddSprite("Icon28", "Images/Loot/Icon28");
        SpriteManager.AddSprite("Icon29", "Images/Loot/Icon29");
        SpriteManager.AddSprite("Icon30", "Images/Loot/Icon30");
        SpriteManager.AddSprite("Icon31", "Images/Loot/Icon31");
        SpriteManager.AddSprite("Icon32", "Images/Loot/Icon32");
        SpriteManager.AddSprite("Icon33", "Images/Loot/Icon33");
        SpriteManager.AddSprite("Icon34", "Images/Loot/Icon34");
        SpriteManager.AddSprite("Icon35", "Images/Loot/Icon35");
        SpriteManager.AddSprite("Icon36", "Images/Loot/Icon36");
        SpriteManager.AddSprite("Icon37", "Images/Loot/Icon37");
        SpriteManager.AddSprite("Icon38", "Images/Loot/Icon38");
        SpriteManager.AddSprite("Icon39", "Images/Loot/Icon39");
        SpriteManager.AddSprite("Icon41", "Images/Loot/Icon41");
        SpriteManager.AddSprite("Icon42", "Images/Loot/Icon42");
        SpriteManager.AddSprite("Icon43", "Images/Loot/Icon43");
        SpriteManager.AddSprite("Icon44", "Images/Loot/Icon44");
        SpriteManager.AddSprite("Icon45", "Images/Loot/Icon45");
        SpriteManager.AddSprite("Icon46", "Images/Loot/Icon46");
        SpriteManager.AddSprite("Icon47", "Images/Loot/Icon47");
        SpriteManager.AddSprite("Icon48", "Images/Loot/Icon48");
        
        SpriteManager.AddSprite("Coin", "Images/Icons/Coin");
    }
    
    private T Add<T>(T obj)
    {
        if (obj is IUpdatable updatable)
            updatables.Add(updatable);

        if (obj is IDrawable drawable)
            drawables.Add(drawable);

        if (obj is IHasCollider hasCollider && hasCollider.Collider != null)
            colliders.Add(hasCollider.Collider);

        return obj;
    }

    private void Remove<T>(T obj)
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
    
    private void CreateProps()
    {
        CreateProp(PropFactory,"crypt",  new Vector2(1300, 350));
        CreateProp(PropFactory,"tree",  new Vector2(1600, 700));
        CreateProp(PropFactory,"tree",  new Vector2(800, 800));
        CreateProp(PropFactory,"dirt",  new Vector2(500, 800));
        CreateProp(PropFactory,"spade",  new Vector2(1300, 820));
        
        Prop angel = CreateProp(PropFactory,"angel",  new Vector2(1600, 250));
        //angel.Interaction = new TraderInteraction(angel);
        //interactionSystem.RegisterInteraction(angel.Interaction);
    }

    private T CreateProp<T>(Func<string, T> factory, string name, Vector2 position) where T : Prop
    {
        T prop = factory(name);
        prop.Transform.Position = position;
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
        prop.Start();
        Add(prop);
        props.Add(prop);
        return prop;
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
        Prop lamp = CreateProp<Prop>(PropFactory,"lampost", position);
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
        TombstoneData tombstoneData = TombstoneGenerator.GenerateTombstoneData(gameContext.RandomService);
    
        Prop earth = CreateProp(PropFactory, "grave_earth", position);
    
        Vector2 tombstonePosition = new Vector2(position.X, position.Y - 190);
        Tombstone tomb = CreateProp(TombstoneFactory, name, tombstonePosition);
        tomb.Transform.Scale = new Vector2(0.3f, 0.3f);
        tomb.SetData(tombstoneData);
        tomb.State = gameContext.RandomService.RandomEnum<TombstoneState>();
    
        tomb.GraveTile = earth;
    
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

    
    public void SpawnLoot(List<ItemData> loot, Tombstone tombstone)
    {
        Console.WriteLine("Looted " + loot.Count + " items");
        foreach (ItemData item in loot)
        {
            ItemPickUp itemPickUp = CreateProp(LootFactory, item.SpriteName, tombstone.Transform.Position);
            itemPickUp.Transform.Scale = new Vector2(0.3f, 0.3f);
            itemPickUp.SetData(item);
            
            PickUpInteraction interaction = new PickUpInteraction(itemPickUp);
            interaction.OnItemPickedUp += PickUpItem;
            itemPickUp.Interaction = interaction;
            InteractionSystem.RegisterInteraction(interaction);
            
            Vector2 origin = new Vector2(tombstone.Left + tombstone.Width * 0.5f,
                                        tombstone.Bottom);

            Point itemSize = new(
                itemPickUp.destRectangle.Width,
                itemPickUp.destRectangle.Height
            );

            Vector2? position = LootPlacementService.FindFreePosition(
                origin, itemSize,
                props.Select(prop => prop.GetDestRectangle(prop.sourceRectangle)).ToArray()
            );

            if (position.HasValue)
            {
                itemPickUp.Transform.Position = position.Value;
            }
            
            Console.WriteLine(item.ToString());
        }
    }

    private void PickUpItem(ItemPickUp pickable)
    {
        Console.WriteLine("Picking up " + pickable.ItemData.Name);
        gameplayActions.PickupItem(pickable.ItemData);
        RemoveProp(pickable);
    }

    private void RemoveProp(ItemPickUp pickable)
    {
        InteractionSystem.UnregisterInteraction(pickable.Interaction);
        Remove(pickable);
        props.Remove(pickable);
    }
    
    private void SpawnGraveDirt(Tombstone tombstone)
    {
        if (tombstone.GraveTile != null)
        {
            tombstone.GraveTile.ChangeSprite("grave_digged");
        }

        if (tombstone.GraveTile != null)
        {
            Vector2 dirtPosition = new Vector2(tombstone.GraveTile.Transform.Position.X, tombstone.GraveTile.Transform.Position.Y + 140);
            CreateProp(PropFactory, "dirt", dirtPosition);
        }
    }

    private void RemoveGraveDirt(Tombstone tombstone)
    {
        if (tombstone.GraveTile != null)
        {
            tombstone.GraveTile.ChangeSprite("grave_earth");
        }

        if (tombstone.GraveTile != null)
        {
            Vector2 dirtPosition = new Vector2(tombstone.GraveTile.Transform.Position.X, tombstone.GraveTile.Transform.Position.Y + 140);
            Prop dirtToRemove = props.FirstOrDefault(p => p.sourceRectangle != null && p.Transform.Position == dirtPosition);
        
            if (dirtToRemove != null)
            {
                Remove(dirtToRemove);
                props.Remove(dirtToRemove);
            }
        }
    }
}