using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.Core;

public class SpriteSheet
{
    public int Rows { get; private set; }
    public int Columns { get; private set; }
    public Texture2D Texture { get; private set; }

    public SpriteSheet(Texture2D texture, int columns = 1, int rows = 1)
    {
        Rows = rows;
        Columns = columns;
        Texture = texture;
    }

    public Rectangle this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= Columns)
                throw new ArgumentOutOfRangeException(nameof(x));

            if (y < 0 || y >= Rows)
                throw new ArgumentOutOfRangeException(nameof(y));
            
            int cellWidth = (int) (Texture.Width * (1f / Columns));
            int cellHeight = (int) (Texture.Height * (1f / Rows));
            
            int sourceX = (int)(Texture.Width * ((float)x /Columns));
            int sourceY =  (int)(Texture.Height * ((float)y /Rows));

            return new Rectangle(
                sourceX,
                sourceY,
                cellWidth,
                cellHeight
            );
        }
    }
}