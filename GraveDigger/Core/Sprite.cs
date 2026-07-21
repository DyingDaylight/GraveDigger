using System;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GraveDigger.Core;

public class Sprite : IDrawable, IUpdatable
{
    private Rectangle? sourceRectangle;
    
    public float Width => SourceRectangle?.Width * Transform.Scale.X ?? 0f;
    public float Height => SourceRectangle?.Height * Transform.Scale.Y ?? 0f;

    public float Left => Transform.Position.X - Origin.X * Transform.Scale.X;
    public float Right => Left + Width;
    public float Top => Transform.Position.Y - Origin.Y * Transform.Scale.Y;
    public float Bottom => Top + Height;
    
    public Vector2 Pivot { get; set; } = new(0.5f, 0.5f);

    public Vector2 Origin => SourceRectangle.HasValue 
        ? new Vector2(
            SourceRectangle.Value.Width * Pivot.X, 
            SourceRectangle.Value.Height * Pivot.Y)
         : Vector2.Zero;

    public Transform Transform { get; } = new();
    public Color Color { get; set; } = Color.White;
    public float Opacity { get; set; } = 1f;
    public float SortingOrder { get; set; }
    public SpriteEffects SpriteEffect { get; set; } = SpriteEffects.None;

    public bool Highlighted { get; set; }
    public Color HighlightColor { get; set; } = Color.Red;
    public int HighlightThickness { get; set; } = 7;
    
    public bool CastShadow { get; set; } = false;
    public float ShadowOpacity { get; set; } = 0.33f;
    public float ShadowScaleY { get; set; } = 0.13f;
    public float ShadowOffsetY { get; set; } = -20f;
    
    
    public SpriteSheet SpriteSheet { get; private set; }

    public Rectangle? SourceRectangle
    {
        get => sourceRectangle;
        set => sourceRectangle = value;
    }
    
    // Kept as a calculated property so it is always up to date.
    public Rectangle DestRectangle => GetDestRectangle(SourceRectangle);
    
    public Texture2D Texture => SpriteSheet.Texture;

    public bool Visible { get; set; } = true;
    
    public Sprite(string name)
    {
        ChangeSprite(name);
    }
    
    public virtual void Start()
    {
        SourceRectangle = SpriteSheet[0, 0];
    }

    public virtual void Update(GameTime gameTime)
    {
        //DestRectangle = GetDestRectangle(SourceRectangle);
    }
    
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (SpriteSheet == null)
            return;

        if (CastShadow)
            DrawShadow(spriteBatch);
        
        if (Highlighted)
            DrawHighlight(spriteBatch);
        
        DrawSprite(
            spriteBatch,
            Transform.Position,
            Color * Opacity,
            Transform.Scale,
            SpriteEffect,
            SortingOrder,
            MathHelper.ToRadians(Transform.Rotation),
            Origin);
    }
    
    public Rectangle GetDestRectangle(Rectangle? source)
    {
        if (source == null)
            return Rectangle.Empty;
        
        int width = (int) (source.Value.Width * Transform.Scale.X);
        int height = (int) (source.Value.Height * Transform.Scale.Y);

        int x = (int) (Transform.Position.X - Origin.X * Transform.Scale.X);
        int y = (int) (Transform.Position.Y - Origin.Y * Transform.Scale.Y);
        
        return new Rectangle(x, y, width, height);
    }
    
    public void ChangeSprite(string spriteName)
    {
        SpriteSheet spriteSheet = SpriteManager.GetSprite(spriteName);
        
        if (spriteSheet == null)
            throw new InvalidOperationException(
                $"Sprite '{spriteName}' was not found in SpriteManager.");
        
        SpriteSheet = spriteSheet;
        SourceRectangle = SpriteSheet[0, 0];
    }
    
    private void DrawHighlight(SpriteBatch spriteBatch)
    {
        Vector2[] offsets =
        {
            new(-HighlightThickness, 0),
            new(HighlightThickness, 0),
            new(0, -HighlightThickness),
            new(0, HighlightThickness)
        };
        
        foreach (Vector2 offset in offsets)
        {
            DrawSprite(
                spriteBatch,
                Transform.Position + offset,
                HighlightColor,
                Transform.Scale,
                SpriteEffect,
                SortingOrder + 0.001f,
                MathHelper.ToRadians(Transform.Rotation), 
                Origin);
        }
    }
    
    private void DrawShadow(SpriteBatch spriteBatch)
    {
        if (!SourceRectangle.HasValue)
            return;
        
        Rectangle source = SourceRectangle.Value;
        Rectangle bounds = GetDestRectangle(source);
        
        SpriteEffects shadowEffect =
            SpriteEffect == SpriteEffects.FlipHorizontally
                ? SpriteEffects.None
                : SpriteEffects.FlipHorizontally;
        
        Vector2 shadowPosition = new(Transform.Position.X, bounds.Bottom + ShadowOffsetY);
        
        Vector2 shadowOrigin = new(source.Width * Pivot.X, source.Height);
        
        Vector2 shadowScale = new(Transform.Scale.X, Transform.Scale.Y * ShadowScaleY);
        
        DrawSprite(
            spriteBatch,
            shadowPosition,
            Color.Black * ShadowOpacity,
            shadowScale,
            shadowEffect,
            SortingOrder + 0.0001f,
            MathHelper.Pi,
            shadowOrigin);
    }
    
    private void DrawSprite(
        SpriteBatch spriteBatch,
        Vector2 position,
        Color tint,
        Vector2 scale,
        SpriteEffects effects,
        float sortingOrder,
        float rotation,
        Vector2 origin)
    {
        if (!Visible)
            return;
        
        spriteBatch.Draw(
            Texture,
            position,
            SourceRectangle,
            tint,
            rotation,
            origin,
            scale,
            effects,
            sortingOrder);
    }
}