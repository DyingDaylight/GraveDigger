using System;
using GUI;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GraveDigger.GUI.Elements;

public class Label : UIElement
{
    private Vector2 position;
    private string text = "";

    public string Text
    {
        get => text;
        set => text = value ?? "";
    }
    public Color Color { get; set; } = Color.White;

    public SpriteFont Font { get; set; }

    public Rectangle Bounds
    {
        get
        {
            return new Rectangle((int)position.X, (int)position.Y, 
                (int)Font.MeasureString(Text).X, (int)Font.MeasureString(Text).Y);
        }
    }

    public Label()
    {
        Font = GUIResources.DefaultFont;
    }
    
    public override void Start()
    {
        base.Start();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Font == null || string.IsNullOrEmpty(Text))
            return;
        
        spriteBatch.DrawString(
            Font,
            Text,
            position, 
            Color,
            MathHelper.ToRadians(0),
            Font.MeasureString(Text) * 0.5f,
            1,
            SpriteEffects.None,
            0);
    }

    // Positions the label in the center of the given rectangle.
    public void CenterIn(Rectangle bounds)
    {
        position = new Vector2(bounds.X + bounds.Width * 0.5f, 
                               bounds.Y + bounds.Height * 0.5f);
    }
    
    public override void SetPosition(int x, int y)
    {
        position = new Vector2(x, y);
    }
}