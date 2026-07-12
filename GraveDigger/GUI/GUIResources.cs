using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GUI;

public static class GUIResources
{
    public static Texture2D ButtonDefaultTexture { get; private set; }
    public static SpriteFont DefaultFont { get; private set; }
    
    public static readonly Color ButtonNormalColor = Color.White;
    public static readonly Color ButtonHoverColor = Color.DarkSeaGreen;
    public static readonly Color ButtonPressedColor = Color.ForestGreen;
    public static readonly Color ButtonDisabledColor = Color.Gray;
    
    private static bool loaded;
    
    public static void LoadContent(ContentManager content)
    {
        if (loaded)
            return;

        loaded = true;
        
        ButtonDefaultTexture = content.Load<Texture2D>("Images/GUI/Button");
        DefaultFont = content.Load<SpriteFont>("Fonts/File");
    }
}