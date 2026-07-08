using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
    

    public void Start()
    {
        CreateMap();
        CreateProps();
        CreateLamps();
        CreateTombstones();
    }

    public void Update(GameTime gameTime)
    {
        map.Update(gameTime);
        foreach (Prop prop in props)
        {
            prop.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        map.Draw(spriteBatch);
        foreach (Prop prop in props)
        {
            prop.Draw(spriteBatch);
        }
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
    
    private void CreateMap()
    {
        map.TileMap = tileMapSchema;
        map.Start();
    }    
    
    private void CreateProps()
    {
        CreateProp("crypt",  new Vector2(1300, 350));
        CreateProp("angel",  new Vector2(1600, 250));
        CreateProp("tree",  new Vector2(1600, 700));
        CreateProp("tree",  new Vector2(800, 800));
        CreateProp("dirt",  new Vector2(1300, 800));
        CreateProp("spade",  new Vector2(1300, 820));
    }

    private Prop CreateProp(string name, Vector2 position)
    {
        Prop prop = new Prop(name);
        prop.Transform.Position = position;
        prop.Start();
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
        Prop lamp = CreateProp("lampost", position);
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
        Prop tomb = CreateProp(name, position);
        tomb.Transform.Scale = new Vector2(0.8f, 0.8f);
    }
}