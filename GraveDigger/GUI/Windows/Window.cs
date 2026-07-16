using GraveDigger.Core;
using GraveDigger.GUI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUI.Windows;

public abstract class Window : UIContainer
{
    protected readonly QuantitySelector quantitySelector;
    protected readonly ContextMenu contextMenu;
    
    protected Window(Rectangle parentBounds)
    {
        int width = 1000;
        int height = 800;
        int x = (int) (parentBounds.X + (parentBounds.Width - width) * 0.5f);
        int y = (int) (parentBounds.Y + (parentBounds.Height - height) * 0.5f);
        Bounds = new Rectangle(x, y, width, height);
        
        Color = Color.White;
        Texture = SpriteManager.GetSprite("background").Texture;
        
        contextMenu = new ContextMenu();
        quantitySelector = new QuantitySelector(Bounds);
        quantitySelector.ConfirmRequested += HandleQuantityConfirmed;
    }

    public override void Update(GameTime gameTime)
    {
        if (quantitySelector.Visible)
        {
            quantitySelector.Update(gameTime);
            return;
        }

        base.Update(gameTime);

        if (contextMenu.Visible)
            contextMenu.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        if (contextMenu.Visible)
            contextMenu.Draw(spriteBatch);
        if (quantitySelector.Visible)
            quantitySelector.Draw(spriteBatch);
    }

    protected virtual void HandleQuantityConfirmed(int amount)
    {
       
    }
}