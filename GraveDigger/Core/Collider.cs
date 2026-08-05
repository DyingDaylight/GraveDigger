using System;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GraveDigger.Core;

public class Collider : IUpdatable, IDrawable
{
    public bool IsTrigger { get; set; }
    public bool IsActive { get; set; } = true;
    
    public CollisionLayer Layer { get; set; } = CollisionLayer.None;
    public CollisionLayer Mask { get; set; } = CollisionLayer.None;
    
    public Sprite Parent { get; private set; }
    private Rectangle Bounds { get; set; }
    private int Thickness { get; set; } = 1;
    private Color Color { get; set; } = Color.White;
    private Texture2D DebugTexture => SpriteManager.GetSprite("pixel").Texture;
    
    
    public Action<Collider, Collider> Triggered;
    public Action<Collider, Collider> Collided;
    

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
    
    public bool CanCollide(Collider other)
    {
        return IsActive && other.IsActive && 
               (Mask & other.Layer) != 0 &&
               (other.Mask & Layer) != 0;
    }
    
    public bool Intersect(Collider other)
    {
        return Bounds.Intersects(other.Bounds);
    }
    
    public void Notify(Collider selfCollider, Collider otherCollider)
    {
        if (IsTrigger || otherCollider.IsTrigger)
            Triggered?.Invoke(selfCollider, otherCollider);
        else
            Collided?.Invoke(selfCollider, otherCollider);
    }
}