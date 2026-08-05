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
    
    // TODO:
    // SizeRatio is a deliberate simplification.
    // The collider is scaled relative to the sprite bounds and anchored to the
    // bottom-center of the sprite.
    // A more flexible solution would support configurable hitboxes
    // (offset + size) instead of relying on a single scale ratio.
    public Vector2 SizeRatio { get; set; } = Vector2.One;
    
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
        Rectangle parentBounds = Parent.DestRectangle;

        int width = (int)(parentBounds.Width * SizeRatio.X);
        int height = (int)(parentBounds.Height * SizeRatio.Y);

        Bounds = new Rectangle(
            parentBounds.Center.X - width / 2,
            parentBounds.Bottom - height,
            width,
            height
        );
        
        //Bounds = Parent.DestRectangle;
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