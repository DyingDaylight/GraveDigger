using System;
using GraveDigger.Core;
using GraveDigger.GUI.Components;
using GraveDigger.GUI.Elements;

namespace GUI;

public class HUD : UIContainer
{
    private readonly SliderBar reputationView;
    private readonly SliderBar hungerView;
    
    private readonly Button inventoryButton;

    public event Action InventoryRequested;
    
    public HUD(GameContext gameContext)
    {
        reputationView = CreateElement<SliderBar>();
        reputationView.SetLeftIcon(SpriteManager.GetSprite("ReputationBadIcon").Texture);
        reputationView.SetRightIcon(SpriteManager.GetSprite("ReputationGoodIcon").Texture);
        
        hungerView = CreateElement<SliderBar>();
        hungerView.SetPosition(0, 100);
        hungerView.SetLeftIcon(SpriteManager.GetSprite("Hunger").Texture);
        
        inventoryButton = CreateElement<Button>(Button.UiButtonMode.Texture);
        inventoryButton.SetTextures(SpriteManager.GetSprite("InventoryButtonNormal").Texture,
            SpriteManager.GetSprite("InventoryButtonHover").Texture,
        SpriteManager.GetSprite("InventoryButtonPressed").Texture,
        SpriteManager.GetSprite("InventoryButtonDisabled").Texture);
        inventoryButton.SetSize(80, 80);
        inventoryButton.SetPosition((int) gameContext.ScreenSize.X - 100,
            (int) gameContext.ScreenSize.Y - 100);
        inventoryButton.OnClick += OpenInventory;
    }

    public void UpdateReputation(int value, int min, int max)
    {
        reputationView.UpdateValue(value, min, max);
    }
    
    public void UpdateHunger(int value, int min, int max)
    {
        hungerView.UpdateValue(value, min, max);
    }
    
    private void OpenInventory()
    {
        InventoryRequested?.Invoke();
    }
}