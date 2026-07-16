using System;
using GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.GUI.Elements;

public class Label : UIElement
{
    private Vector2 centerPosition;
    private string text = "";
    private SpriteFont font;

    public float Scale { get; set; } = 1;

    public string Text
    {
        get => text;
        set
        {
            text = value ?? "";
            UpdateBounds();
        }
    }

    public SpriteFont Font
    {
        get => font;
        set
        {
            font = value;
            UpdateBounds();
        }
    }

    public Label()
    {
        Font = GUIResources.DefaultFont;
        UpdateBounds();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Font == null || string.IsNullOrEmpty(Text))
            return;
        
        Vector2 textSize = Font.MeasureString(Text);
        
        spriteBatch.DrawString(
            Font,
            Text,
            centerPosition,
            Color,
            0f,
            textSize  * 0.5f,
            Scale,
            SpriteEffects.None,
            0f);
    }

    // Positions the top-left corner of the visible text.
    public override void SetPosition(int x, int y)
    {
        centerPosition = new Vector2(
            x + Bounds.Width * 0.5f,
            y + Bounds.Height * 0.5f);
        UpdateBounds();
    }

    // Centers the label inside the given bounds.
    public void CenterIn(Rectangle containerBounds)
    {
        centerPosition = new Vector2(
            containerBounds.Center.X,
            containerBounds.Center.Y);
        UpdateBounds();
    }

    private void UpdateBounds()
    {
        Vector2 textSize = font?.MeasureString(text) * Scale ?? Vector2.Zero;
        
        Bounds = new Rectangle(
            (int)(centerPosition.X - textSize.X * 0.5f),
            (int)(centerPosition.Y - textSize.Y * 0.5f),
            (int)textSize.X,
            (int)textSize.Y);
    }
}