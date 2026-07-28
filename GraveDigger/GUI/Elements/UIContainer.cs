using System;
using System.Collections.Generic;
using GraveDigger.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.GUI.Elements;

public abstract class UIContainer : UIElement
{
    protected readonly List<UIElement> elements = new();
    
    public override void Start()
    {
        base.Start();
        
        foreach (var element in elements) 
            element.Start();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        if (!IsEnabled)
            return;

        foreach (var element in elements)
        {
            if (element.Visible)
                element.Update(gameTime);
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        foreach (var element in elements)
        {
            if (element.Visible)
                element.Draw(spriteBatch);
        }
    }

    // to create elements without parameters
    protected T CreateElement<T>() where T : UIElement, new()
    {
        T element = new T();
        elements.Add(element);
        
        return element;
    }
    
    // to create elements with parameters
    protected T CreateElement<T>(params object[] args) where T : UIElement
    {
        T element = (T)System.Activator.CreateInstance(typeof(T), args);
        elements.Add(element);
        return element;
    }
	
	protected void RemoveElement<T>(T element) where T : UIElement, new()
    {
        elements.Remove(element);
	}
    
    protected void AddElement(UIElement element)
    {
        elements.Add(element);
    }
    
    protected Button CreateButton(string text, int width, int height, Action onClick)
    {
        var btn = CreateElement<Button>(Button.UiButtonMode.Texture);
        btn.SetTextures(
            SpriteManager.GetSprite("ButtonMainMenu").Texture,
            SpriteManager.GetSprite("ButtonHover").Texture,
            SpriteManager.GetSprite("ButtonPressed").Texture,
            SpriteManager.GetSprite("ButtonDisabled").Texture
        );
        btn.LockSize(width, height);
        btn.SetText(text);
        btn.OnClick += onClick;
        btn.Start();
        return btn;
    }
}