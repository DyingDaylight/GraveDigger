using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IDrawable = Interfaces.IDrawable;

namespace GraveDigger.GUI.Elements;

// Base class for all visible UI elements.
public abstract class UIElement : IUpdatable, IDrawable, ILayoutElement
{
    public Texture2D? Texture { get; protected set; }
    public Rectangle Bounds { get; protected set; }
    public Color Color { get; set; } = Color.White;
    
    public virtual Vector2 VisibleSize =>  new(Bounds.Width, Bounds.Height);

    public virtual void Start()
    {
    }

    public virtual void Update(GameTime gameTime)
    {
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (Texture == null) return;
        
        spriteBatch.Draw(Texture, Bounds, Color);
    }

    public virtual void SetPosition(int x, int y)
    {
        Bounds = new Rectangle(x, y, Bounds.Width, Bounds.Height);
    }

    public virtual void SetSize(int width, int height)
    {
        Bounds = new Rectangle(Bounds.X, Bounds.Y, width, height);
    }
}