using System;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GraveDigger;

public class Sprite : IDrawable, IUpdatable
{
    public float Width => sourceRectangle.Value.Width * Transform.Scale.X;
    public float Height => sourceRectangle.Value.Height * Transform.Scale.Y;

    public float Left => Transform.Position.X - Origin.X * Transform.Scale.X;
    public float Right => Left + Width;
    public float Top => Transform.Position.Y - Origin.Y * Transform.Scale.Y;
    public float Bottom => Top + Height;
    
    public Vector2 Pivot { get; set; } = new Vector2(0.5f, 0.5f);

    public Vector2 Origin
    {
        get
        {
            return new Vector2(sourceRectangle.Value.Width * Pivot.X, sourceRectangle.Value.Height * Pivot.Y);
        }
    }

    public Transform Transform { get; } = new();
    public Color Color { get; set; } = Color.White;
    public float SortingOrder { get; set; } = 0;
    public SpriteEffects SpriteEffect { get; set; } = SpriteEffects.None;

    public bool Highlighted { get; set; } = false;
    public Color HighlightColor { get; set; } = Color.Red;
    public int HighlightThickness { get; set; } = 7;
    
    public SpriteSheet SpriteSheet;
    public Rectangle? sourceRectangle;
    public Rectangle destRectangle;
    
    public Texture2D Texture
    {
        get { return SpriteSheet.Texture; }
    }
    
    public Sprite(string name)
    {
        ChangeSprite(name);
    }
    
    public virtual void Start()
    {
        sourceRectangle = SpriteSheet[0, 0];
        //Origin = new Vector2(sourceRectangle.Value.Width * 0.5f, sourceRectangle.Value.Height * 0.5f);
    }

    public virtual void Update(GameTime gameTime)
    {
        //Origin = new Vector2(sourceRectangle.Value.Width * 0.5f, sourceRectangle.Value.Height * 0.5f);
        destRectangle = GetDestRectangle(sourceRectangle);
    }
    
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (Texture == null)
            return;

        if (Highlighted)
        {
            DrawHighlight(spriteBatch);
        }
        
        spriteBatch.Draw(
            Texture, 
            Transform.Position,
            sourceRectangle,
            Color,
            MathHelper.ToRadians(Transform.Rotation),
            Origin,
            Transform.Scale,
            SpriteEffect,
            SortingOrder);
    }

    private void DrawHighlight(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            Texture, 
            Transform.Position + new Vector2(-HighlightThickness, 0),
            sourceRectangle,
            HighlightColor,
            MathHelper.ToRadians(Transform.Rotation),
            Origin,
            Transform.Scale,
            SpriteEffect,
            SortingOrder + 0.001f);
        
        spriteBatch.Draw(
            Texture, 
            Transform.Position + new Vector2(HighlightThickness, 0),
            sourceRectangle,
            HighlightColor,
            MathHelper.ToRadians(Transform.Rotation),
            Origin,
            Transform.Scale,
            SpriteEffect,
            SortingOrder + 0.001f);
        
        spriteBatch.Draw(
            Texture, 
            Transform.Position + new Vector2(0,HighlightThickness),
            sourceRectangle,
            HighlightColor,
            MathHelper.ToRadians(Transform.Rotation),
            Origin,
            Transform.Scale,
            SpriteEffect,
            SortingOrder + 0.001f);
        
        spriteBatch.Draw(
            Texture, 
            Transform.Position + new Vector2(0, -HighlightThickness),
            sourceRectangle,
            HighlightColor,
            MathHelper.ToRadians(Transform.Rotation),
            Origin,
            Transform.Scale,
            SpriteEffect,
            SortingOrder + 0.001f);
    }
    
    public Rectangle GetDestRectangle(Rectangle? source)
    {
        if (source == null)
            return new Rectangle();
        
        int width = (int) (source.Value.Width * Transform.Scale.X);
        int height = (int) (source.Value.Height * Transform.Scale.Y);

        int x = (int) (Transform.Position.X - Origin.X * Transform.Scale.X);
        int y = (int) (Transform.Position.Y - Origin.Y * Transform.Scale.Y);
        
        return new Rectangle(x, y, width, height);
    }
    
    protected void ChangeSprite(string spriteName)
    {
        SpriteSheet = SpriteManager.GetSprite(spriteName);
    }
}