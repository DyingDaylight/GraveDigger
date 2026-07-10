using System;
using System.Collections.Generic;
using GraveDigger.Interactions;
using GraveDigger.Props;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GraveDigger;

public class Level
{
    private const int MapTilesAmount = 9;
    private readonly int[,] tileMapSchema =
    {
        { 0, 2, 0, 1, 0, 2, 3, 7, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 4, 0, 0, 2, 8, 0, 0, 2, 2, 0, 0, 0 },
        { 0, 5, 6, 5, 0, 0, 2, 7, 0, 0, 0, 0, 0, 5, 5 },
        { 0, 3, 2, 5, 0, 2, 6, 7, 0, 0, 0, 0, 0, 5, 5 },
        { 8, 7, 8, 8, 7, 7, 7, 7, 7, 8, 7, 7, 7, 8, 7 },
        { 0, 3, 0, 5, 1, 2, 2, 7, 0, 0, 0, 1, 0, 0, 0 },
        { 0, 2, 6, 2, 0, 3, 2, 7, 0, 0, 0, 0, 1, 0, 0 },
        { 0, 1, 3, 2, 4, 2, 2, 7, 0, 6, 0, 2, 0, 3, 4 },
        { 0, 3, 6, 5, 0, 5, 2, 7, 5, 4, 3, 0, 0, 0, 2 },
    };
    
    private List<Prop> props = new List<Prop>();
    private Map map = new Map();
    
    private List<IUpdatable> updatables = new();
    private List<IDrawable> drawables = new();
    private List<Collider> colliders = new();
    private List<Interaction> interactions = new();

    public InteractionSystem InteractionSystem;
    
    private GameContext gameContext;

    public Level(GameContext gameContext)
    {
        this.gameContext = gameContext;
    }
    
    public void Start()
    {
        InteractionSystem = new InteractionSystem(gameContext.CoordinatesConverter);
        
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
        
        //foreach (Interaction interaction in interactions)
        //    interaction.Draw(spriteBatch);
    }
    
    public void LoadTextures()
    {
        for (int i = 0; i < MapTilesAmount; i++)
        {
            SpriteManager.AddSprite($"ground{i}", $"Images/Ground/ground{i}");
        }
        SpriteManager.AddSprite("tree", "Images/Props/Tree");
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
    }
    
    private T Add<T>(T obj)
    {
        if (obj is IUpdatable updatable)
            updatables.Add(updatable);

        if (obj is IDrawable drawable)
            drawables.Add(drawable);

        if (obj is IHasCollider hasCollider && hasCollider.Collider != null)
            colliders.Add(hasCollider.Collider);
        
        if (obj is ICanInteract interactable)
            interactions.Add(interactable.Interaction);

        return obj;
    }

    private void Remove<T>(T obj)
    {
        // TODO: inmplement remove
        // TODO: do not forget about unregestering Interactions
        // TODO: do not forget about colliders
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
        CreateProp(PropFactory,"dirt",  new Vector2(1300, 800));
        CreateProp(PropFactory,"spade",  new Vector2(1300, 820));
        
        Prop angel = CreateProp(PropFactory,"angel",  new Vector2(1600, 250));
        //angel.Interaction = new TraderInteraction(angel);
        //interactionSystem.RegisterInteraction(angel.Interaction);
    }

    private T CreateProp<T>(Func<string, T> factory, string name, Vector2 position) where T : Prop
    {
        T prop = factory(name);
        prop.Transform.Position = position;
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
        Tombstone tomb = CreateProp(TombstoneFactory, name, position);
        tomb.Transform.Scale = new Vector2(0.8f, 0.8f);
        tomb.Interaction = new TombstoneInteraction(tomb);
        InteractionSystem.RegisterInteraction(tomb.Interaction);
    }
    
    private Prop PropFactory(string spriteName)
    {
        return new Prop(spriteName);
    }

    private Tombstone TombstoneFactory(string spriteName)
    {
        return new Tombstone(spriteName);
    }
}