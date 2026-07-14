using System;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GraveDigger.Core;

public class Collider : IUpdatable, IDrawable
{
    public bool IsTrigger { get; set; }
    
    public Color Color { get; set; } = Color.White;
    public int Thickness { get; set; } = 1;
    
    public Rectangle Bounds { get; private set; }
    public Sprite Parent { get; set; }
    
    public Action<Collider, Collider> onTrigger;
    public Action<Collider, Collider> onCollision;
        
    private Texture2D DebugTexture => SpriteManager.GetSprite("pixel").Texture;
    

    public Collider(Sprite parent)
    {
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
    }
    
    public void Start()
    {
        Bounds = Parent.DestRectangle;
    }
    
    public void Update(GameTime gameTime)
    {
        Bounds = Parent.DestRectangle;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
#if DEBUG
        // draw outline bounds
        
        spriteBatch.Draw(
            DebugTexture,
            new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Thickness), // top
            Color);

        spriteBatch.Draw(
            DebugTexture,
            new Rectangle(Bounds.X, Bounds.Y, Thickness, Bounds.Height), // left
            Color);

        spriteBatch.Draw(
            DebugTexture,
            new Rectangle(Bounds.X + Bounds.Width - Thickness, Bounds.Y, Thickness, Bounds.Height), // right
            Color);

        spriteBatch.Draw(
            DebugTexture,
            new Rectangle(Bounds.X, Bounds.Y + Bounds.Height - Thickness, Bounds.Width, Thickness), // bottom
            Color);

#endif
    }
    
    public bool Intersect(Collider other)
    {
        return Bounds.Intersects(other.Bounds);
    }
    
    public void Notify(Collider selfCollider, Collider otherCollider)
    {
        if (IsTrigger || otherCollider.IsTrigger)
            onTrigger?.Invoke(selfCollider, otherCollider);
        else
            onCollision?.Invoke(selfCollider, otherCollider);
    }
}