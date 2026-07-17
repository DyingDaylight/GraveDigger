using System;
using GraveDigger.Core;
using GraveDigger.GUI.Elements;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GUI;

public class HUD : UIContainer
{
    private readonly Button inventoryButton;

    public event Action InventoryRequested;
    
    public HUD(GameContext gameContext)
    {
        inventoryButton = CreateElement<Button>(Button.UiButtonMode.Texture);
        inventoryButton.SetTextures(SpriteManager.GetSprite("InventoryButtonNormal").Texture,
            SpriteManager.GetSprite("InventoryButtonHover").Texture,
        SpriteManager.GetSprite("InventoryButtonPressed").Texture,
        SpriteManager.GetSprite("InventoryButtonDisabled").Texture);
        inventoryButton.SetSize(120, 150);
        inventoryButton.SetPosition((int) gameContext.ScreenSize.X - 160,
            (int) gameContext.ScreenSize.Y - 180);
        inventoryButton.OnClick += OpenInventory;
    }

    public void UpdateReputation(int value)
    {
        // TODO: draw interface on screen
        Console.WriteLine("Reputation: " + value);
    }
    
    private void OpenInventory()
    {
        InventoryRequested?.Invoke();
    }
}