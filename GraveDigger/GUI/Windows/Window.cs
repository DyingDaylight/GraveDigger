using GraveDigger.Core;
using GraveDigger.GUI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUI.Windows;

public abstract class Window : UIContainer
{
    
    protected readonly ContextMenu contextMenu;
    
    protected Window()
    {
        int width = 1000;
        int height = 800;
        int x = (int) ((1920 - width) * 0.5f);
        int y = (int) ((1080 - height) * 0.5f);
        Bounds = new Rectangle(x, y, width, height);
        
        Color = Color.DimGray;
        Texture = SpriteManager.GetSprite("pixel").Texture;
        
        contextMenu = new ContextMenu();
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (contextMenu.Visible)
            contextMenu.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        if (contextMenu.Visible)
            contextMenu.Draw(spriteBatch);
    }
}