using System;
using System.Collections.Generic;
using GraveDigger.Core;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GraveDigger;

public class Map : IDrawable, IUpdatable
{
    private readonly Point tileSize = new(1056, 1056);
    private readonly Point tileOverlap = new(1, 5);
    private readonly List<Sprite> tileSprites = new();
    
    private Point mapSize = Point.Zero;
    
    private bool started;
    
    private int[,] tileMap;
    public int[,] TileMap
    {
        get { return tileMap; }
        // Sets the tile map and updates the map dimensions.
        set
        {
            if (value == null)
                throw new ArgumentNullException("TileMap cannot be null.");
            
            tileMap = value; 
            mapSize.X = tileMap.GetLength(1);
            mapSize.Y = tileMap.GetLength(0);
        }
    }
    
    public void Start()
    {
        // Prevent creating duplicate tile sprites if Start() is called more than once.
        if (started)
            throw new InvalidOperationException("Map has already been started.");

        started = true;
    
        // Create tile sprites from the tile map data.
        for (int i = 0; i < mapSize.Y; i++)
        {
            for (int j = 0; j < mapSize.X; j++)
            {
                int tileIndex = tileMap[i, j];
  
                Sprite tile = new Sprite($"ground{tileIndex}");
            
                tile.Transform.Position = new Vector2(j * (tileSize.X - tileOverlap.X), i * (tileSize.Y - tileOverlap.Y));
            
                tile.SourceRectangle = new Rectangle(0, 0, tileSize.X, tileSize.Y);
            
                tile.Pivot = new Vector2(0, 0);
                tile.SortingOrder = 1f;
                tile.Start();
                
                tileSprites.Add(tile);
            }
        }
    }

    public void Update(GameTime gameTime)
    {
        foreach (Sprite sprite in tileSprites)
            sprite.Update(gameTime);
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (Sprite sprite in tileSprites)
            sprite.Draw(spriteBatch);
    }
}