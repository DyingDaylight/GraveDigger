using System.Collections.Generic;
using GraveDigger;
using GraveDigger.GUI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUI.Windows;

public abstract class Window : UIContainer
{
    public Window()
    {
        int width = 1000;
        int height = 800;
        int x = (int) ((1920 - width) * 0.5f);
        int y = (int) ((1080 - height) * 0.5f);
        Bounds = new Rectangle(x, y, width, height);
        
        Color = Color.DimGray;
        Texture = SpriteManager.GetSprite("pixel").Texture;
    }
}