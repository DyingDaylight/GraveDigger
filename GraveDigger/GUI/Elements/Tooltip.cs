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
        label.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!Visible)
            return;
        
        base.Draw(spriteBatch);
        label.Draw(spriteBatch);
    }
    
    public void SetTooltip(string tooltip)
    {
        label.Text = tooltip;
        SetSize((int) (label.Size.X + padding.X * 2),
            (int) (label.Size.Y + padding.Y * 2));
    }
    
    public void SetFont(SpriteFont font)
    {
        label.Font = font;
        RefreshLayout();
    }

    protected override void RefreshLayout()
    {
        base.RefreshLayout();
        label.CenterIn(Bounds);
    }
}