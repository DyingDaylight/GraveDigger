using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.GUI.Elements;

public class Image : UIElement
{
    public void SetImage(Texture2D texture)
    {
        if (texture == null)
            return;
        
        Texture = texture;
    }

    public void SetTint(Color tint)
    {
        Color = tint;
    }
}