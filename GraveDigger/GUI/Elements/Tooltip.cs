using System;
using GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.GUI.Elements;

public class Tooltip : UIElement
{
    public Vector2 paddings = new Vector2(20, 20);
    private Label label = new Label();
    
    public override void Start()
    {
        SetFont(GUIResources.DefaultFont);
        Texture = SpriteManager.GetSprite("pixel").Texture;
        Color = Color.DimGray;
        base.Start();
    }
    
    public override void Update(GameTime gameTime)
    {
        label.CenterIn(Bounds);
        label.Update(gameTime);
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        label.Draw(spriteBatch);
    }
    
    public void SetTooltip(string tooltip)
    {
        label.Text = tooltip;
        
        Vector2 size = label.Font.MeasureString(tooltip) + paddings;
        SetSize((int) size.X, (int) size.Y);
    }
    
    public void SetFont(SpriteFont font)
    {
        label.Font = font;
    }
}