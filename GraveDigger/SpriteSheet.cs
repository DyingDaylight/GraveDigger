using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger;

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
            int segWidth = (int) (Texture.Width * (1f / Columns));
            int segHeight = (int) (Texture.Height * (1f / Rows));
            
            int pos_x = (int)(Texture.Width * ((float)x /Columns));
            int pos_y =  (int)(Texture.Height * ((float)y /Rows));

            return new Rectangle(
                pos_x,
                pos_y,
                segWidth,
                segHeight
            );
        }
    }
}