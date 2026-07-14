using System.Collections.Generic;
using GraveDigger.Core;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger;

public class SpriteManager
{
    private static ContentManager _content;
    
    static Dictionary<string, SpriteSheet> _spriteSheets = new();

    public SpriteManager(ContentManager content)
    {
        _content = content;
    }

    public static void AddSprite(string name, string fileName, int columns = 1, int rows = 1)
    {
        if (_spriteSheets.ContainsKey(name))
            return;
        
        Texture2D texture = _content.Load<Texture2D>(fileName);
        _spriteSheets[name] = new SpriteSheet(texture, columns, rows);
    }
    
    public static SpriteSheet GetSprite(string name)
    {
        if (_spriteSheets.ContainsKey(name))
            return _spriteSheets[name];
        
        return null;
    }
}