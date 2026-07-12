using System;
using System.Collections.Generic;
using System.Data;
using GraveDigger;
using GraveDigger.GUI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUI.Windows;

public class Window : UIElement
{
    protected List<UIElement> elements = new List<UIElement>();
    
    public Window()
    {
        SetSize(1000, 800);
        SetPosition(460, 140);
        Color = Color.DimGray;
        Texture = SpriteManager.GetSprite("pixel").Texture;
    }

    public override void Start()
    {
        base.Start();
        foreach (var element in elements) 
            element.Start();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        foreach (var element in elements)
            element.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        foreach (var element in elements)
        {
            element.Draw(spriteBatch);
        }
    }

    protected T CreateElement<T>() where T : UIElement, new()
    {
        T element = new T();
        elements.Add(element);
        return element;
    }
}