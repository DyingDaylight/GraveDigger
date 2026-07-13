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
        Bounds = new Rectangle(460, 140, 1000, 800);
        
        Color = Color.DimGray;
        Texture = SpriteManager.GetSprite("pixel").Texture;
    }
}