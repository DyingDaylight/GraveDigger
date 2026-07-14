using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.Core;

public static class SpriteManager
{
    private static ContentManager? _content;
    
    private static readonly Dictionary<string, SpriteSheet> _spriteSheets = new();

    public static void Initialize(ContentManager content)
    {
        _content = content ?? throw new ArgumentNullException(nameof(content));
    }

    public static void AddSprite(string name, string fileName, int columns = 1, int rows = 1)
    {
        if (_content == null)
            throw new InvalidOperationException(
                "SpriteManager must be initialized before loading sprites.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Sprite name cannot be empty.");

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Sprite file name cannot be empty.");
        
        if (_spriteSheets.ContainsKey(name))
            return;
        
        Texture2D texture = _content.Load<Texture2D>(fileName);
        _spriteSheets[name] = new SpriteSheet(texture, columns, rows);
    }
    
    public static SpriteSheet GetSprite(string name)
    {
        if (_spriteSheets.TryGetValue(name, out SpriteSheet sprite))
            return sprite;
        
        return null;
    }
}