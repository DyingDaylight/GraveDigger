using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GraveDigger.GUI.Elements;

// Base class for UI elements that can be updated and drawn.
public class UIElement : IUpdatable, IDrawable
{
    public Texture2D Texture { get; protected set; }
    public Rectangle Bounds { get; protected set; }
    public Color Color { get; protected set; } = Color.White;
    
    public virtual void Start()
    {
        // Intentionally empty. Override in derived UI elements if needed.
    }

    public virtual void Update(GameTime gameTime)
    {
        // Intentionally empty. Override in derived UI elements if needed.
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (Texture == null) return;
        
        spriteBatch.Draw(
            Texture, 
            Bounds,
            null,
            Color,
            MathHelper.ToRadians(0),
            Vector2.Zero,
            SpriteEffects.None,
            0f);
    }
    
    public virtual void SetPosition(int x, int y)
    {
        Bounds = new Rectangle(x, y, Bounds.Width, Bounds.Height);
    }

    public void SetSize(int width, int height)
    {
        Bounds = new Rectangle(Bounds.X, Bounds.Y, width, height);
    }
}