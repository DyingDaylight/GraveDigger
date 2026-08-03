using System;
using GraveDigger.Core;
using GraveDigger.GUI.Components;
using GraveDigger.GUI.Elements;
using GraveDigger.Systems;

namespace GUI;

public class HUD : UIContainer
{
    private readonly SliderBar reputationView;
    private readonly SliderBar dayTimeView;
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
        
        dayTimeView = CreateElement<SliderBar>();
        dayTimeView.SetLeftIcon(SpriteManager.GetSprite("DayIcon").Texture);
        dayTimeView.SetRightIcon(SpriteManager.GetSprite("NightIcon").Texture);
        dayTimeView.SetPosition((int)(gameContext.ScreenSize.X - 630), 0);
        
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
    
    public void UpdateDayTime(float progress)
    {
        const int min = 0;
        const int max = 100;

        int value = Math.Clamp((int)MathF.Round(progress * max), min, max);

        dayTimeView.UpdateValue(value, min, max);
    }
    
    private void OpenInventory()
    {
        InventoryRequested?.Invoke();
    }
}