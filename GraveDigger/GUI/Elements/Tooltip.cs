using System;
using GraveDigger.Core;
using GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.GUI.Elements;

public class Tooltip : UIElement
{
    private readonly Vector2 padding = new(20, 20);
    private readonly Label label = new();
    
    public override void Start()
    {
        base.Start();
        
        Texture = SpriteManager.GetSprite("pixel").Texture;
        Color = Color.DimGray;

        SetFont(GUIResources.DefaultFont);
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        label.CenterIn(Bounds);
        label.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        label.Draw(spriteBatch);
    }
    
    public override void SetPosition(int x, int y)
    {
        base.SetPosition(x, y);
        label.CenterIn(Bounds);
    }
    
    public void SetTooltip(string tooltip)
    {
        label.Text = tooltip;
        
        SetSize((int) (label.VisibleSize.X + padding.X),
            (int) (label.VisibleSize.Y + padding.Y));
        
        label.CenterIn(Bounds);
    }
    
    public void SetFont(SpriteFont font)
    {
        label.Font = font;
        label.CenterIn(Bounds);
    }
}