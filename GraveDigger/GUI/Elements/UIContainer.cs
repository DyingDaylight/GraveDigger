using System.Collections.Generic;
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

    protected T CreateElement<T>() where T : UIElement, new()
    {
        T element = new T();
        elements.Add(element);
        
        return element;
    }

    protected void RemoveElement<T>(T element) where T : UIElement, new()
    {
        elements.Remove(element);
    }
}