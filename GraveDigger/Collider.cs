using System;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger;

public class Collider : Sprite
{
    public bool isTrigger = false;
    
    public Color color = Color.White;
    public int thickness = 1;
    
    public Action<Collider, Collider> onTrigger;
    public Action<Collider, Collider> onCollision;
        
    public Sprite Parent { get; set; }

    public Collider() : base("pixel")
    {
        sourceRectangle = SpriteSheet.Texture.Bounds;
    }
    
    public bool Intersect(Collider other)
    {
        return destRectangle.Intersects(other.destRectangle);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        destRectangle = Parent.destRectangle;

        for (int i = 0; i < SceneManager.colliders.Count; i++) 
        {
            Collider other = SceneManager.colliders[i];
            if (other != this && Intersect(other))
            {
                Notify(this, other);
            }
        }
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        #if DEBUG
        // draw outline bounds
        
        spriteBatch.Draw(
            SpriteSheet.Texture,
            new Rectangle(destRectangle.X, destRectangle.Y, destRectangle.Width, thickness), // top
            color);

        spriteBatch.Draw(
            SpriteSheet.Texture,
            new Rectangle(destRectangle.X, destRectangle.Y, thickness, destRectangle.Height), // left
            color);

        spriteBatch.Draw(
            SpriteSheet.Texture,
            new Rectangle(destRectangle.X + destRectangle.Width - thickness, destRectangle.Y, thickness, destRectangle.Height), // right
            color);

        spriteBatch.Draw(
            SpriteSheet.Texture,
            new Rectangle(destRectangle.X, destRectangle.Y + destRectangle.Height - thickness, destRectangle.Width, thickness), // bottom
            color);

        #endif
    }
    
    public void Notify(Collider selfCollider, Collider otherCollider)
    {
        if (isTrigger || otherCollider.isTrigger)
            onTrigger?.Invoke(selfCollider, otherCollider);
        else
            onCollision?.Invoke(selfCollider, otherCollider);
    }
}