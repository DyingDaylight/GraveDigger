using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GUI;

public static class GUIResources
{
    public static Texture2D ButtonDefaultTexture { get; private set; }
    public static SpriteFont DefaultFont { get; private set; }
    
    public static void LoadContent(ContentManager content)
    {
        ButtonDefaultTexture = content.Load<Texture2D>($"Images/GUI/Button");
        DefaultFont = content.Load<SpriteFont>("Fonts/File");
    }
}