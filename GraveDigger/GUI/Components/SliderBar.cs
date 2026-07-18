using System;
using GraveDigger.Core;
using GraveDigger.GUI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.GUI.Components;

public class SliderBar : UIContainer
{
    private readonly Image leftIcon;
    private readonly Image rightIcon;
    private readonly Image lineIcon;
    private readonly Image sliderIcon;

    
    public SliderBar()
    {
        lineIcon = CreateElement<Image>();
        lineIcon.SetSize(400, 30);
        lineIcon.SetImage(SpriteManager.GetSprite("ReputationLineIcon").Texture);
        
        leftIcon = CreateElement<Image>();
        leftIcon.SetSize(100, 100);
        
        rightIcon = CreateElement<Image>();
        rightIcon.SetSize(100, 120);
        
        sliderIcon = CreateElement<Image>();
        sliderIcon.SetSize(50, 50);
        sliderIcon.SetImage(SpriteManager.GetSprite("ReputationSliderIcon").Texture);
        
        RefreshLayout();
    }

    public void SetLeftIcon(Texture2D icon)
    {
        if (icon == null) return;
        
        leftIcon.SetImage(icon);
    }

    public void SetRightIcon(Texture2D icon)
    {
        if (icon == null) return;
        
        rightIcon.SetImage(icon);
    }
    
    public void UpdateValue(int value, int min, int max)
    {
        if (max <= min)
            return;

        value = Math.Clamp(value, min, max);
        
        float normalized = (value - min) / (float)(max - min);
        
        float sliderCenterX = MathHelper.Lerp(
            lineIcon.Bounds.Left + sliderIcon.Bounds.Width * 0.5f, 
            lineIcon.Bounds.Right - sliderIcon.Bounds.Width * 0.5f,
            normalized);

        int sliderX = (int)(sliderCenterX - sliderIcon.Bounds.Width * 0.5f);

        sliderIcon.SetPosition(sliderX, sliderIcon.Bounds.Y);
    }

    protected override void RefreshLayout()
    {
        base.RefreshLayout();
        
        lineIcon.SetPosition(Bounds.X + 110, Bounds.Y + 50);
        leftIcon.SetPosition(Bounds.X + 30, Bounds.Y + 20);
        rightIcon.SetPosition(Bounds.X + 490, Bounds.Y + 15);
        sliderIcon.SetPosition(Bounds.X + 290, Bounds.Y + 40);
    }
    
}