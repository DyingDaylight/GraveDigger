using GraveDigger.Core;
using GraveDigger.GUI.Elements;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Components;

public class ReputationView : UIContainer
{
    private readonly Image badIcon;
    private readonly Image goodIcon;
    private readonly Image lineIcon;
    private readonly Image sliderIcon;

    
    public ReputationView()
    {
        lineIcon = CreateElement<Image>();
        lineIcon.SetSize(400, 30);
        lineIcon.SetPosition(110, 50);
        lineIcon.SetImage(SpriteManager.GetSprite("ReputationLineIcon").Texture);
        
        badIcon = CreateElement<Image>();
        badIcon.SetSize(100, 100);
        badIcon.SetPosition(30, 20);
        badIcon.SetImage(SpriteManager.GetSprite("ReputationBadIcon").Texture);
        
        goodIcon = CreateElement<Image>();
        goodIcon.SetSize(100, 120);
        goodIcon.SetPosition(490, 15);
        goodIcon.SetImage(SpriteManager.GetSprite("ReputationGoodIcon").Texture);
        
        sliderIcon = CreateElement<Image>();
        sliderIcon.SetSize(50, 50);
        sliderIcon.SetPosition(290, 40);
        sliderIcon.SetImage(SpriteManager.GetSprite("ReputationSliderIcon").Texture);
    }

    public void UpdateReputation(int value, int min, int max)
    {
        float normalized = (value - min) / (float)(max - min);
        
        float sliderCenterX = MathHelper.Lerp(lineIcon.Bounds.Left, lineIcon.Bounds.Right,
            normalized);

        int sliderX = (int)(sliderCenterX - sliderIcon.Bounds.Width * 0.5f);

        sliderIcon.SetPosition(sliderX, sliderIcon.Bounds.Y);
    }
}