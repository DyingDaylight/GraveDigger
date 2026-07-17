using System;
using GraveDigger.Core;
using GraveDigger.GUI.Components;
using GraveDigger.GUI.Elements;

namespace GUI;

public class HUD : UIContainer
{
    private readonly ReputationView reputationView;
    
    private readonly Button inventoryButton;

    public event Action InventoryRequested;
    
    public HUD(GameContext gameContext)
    {
        reputationView = CreateElement<ReputationView>();
        
        inventoryButton = CreateElement<Button>(Button.UiButtonMode.Texture);
        inventoryButton.SetTextures(SpriteManager.GetSprite("InventoryButtonNormal").Texture,
            SpriteManager.GetSprite("InventoryButtonHover").Texture,
        SpriteManager.GetSprite("InventoryButtonPressed").Texture,
        SpriteManager.GetSprite("InventoryButtonDisabled").Texture);
        inventoryButton.SetSize(80, 100);
        inventoryButton.SetPosition((int) gameContext.ScreenSize.X - 160,
            (int) gameContext.ScreenSize.Y - 180);
        inventoryButton.OnClick += OpenInventory;
    }

    public void UpdateReputation(int value, int min, int max)
    {
        reputationView.UpdateReputation(value, min, max);
    }
    
    private void OpenInventory()
    {
        InventoryRequested?.Invoke();
    }
}