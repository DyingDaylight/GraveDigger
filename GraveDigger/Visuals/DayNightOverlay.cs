using GraveDigger.Core;
using GraveDigger.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GraveDigger.Visuals;

public class DayNightOverlay : IDrawable
{
    private const float TransitionDuration = 0.15f;
    
    private readonly Color dawnColor = new(130, 92, 98, 28);
    private readonly Color dayColor = Color.Transparent;
    private readonly Color sunsetColor = new(110, 85, 95, 55);
    private readonly Color duskColor = new(50, 45, 70, 90);
    private readonly Color nightColor = new(20, 30, 70, 130);
    
    private readonly Texture2D dayNightOverlay;
    private readonly TimeSystem timeSystem;
    private readonly Rectangle bounds;
    
    public DayNightOverlay(TimeSystem timeSystem, Vector2 screenSize)
    {
        this.timeSystem = timeSystem;
        dayNightOverlay = SpriteManager.GetSprite("pixel").Texture;
        bounds = new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y);
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        Color overlayColor = GetDayNightOverlayColor();
        
        spriteBatch.Draw(dayNightOverlay, bounds, overlayColor);
    }
    
    private Color GetDayNightOverlayColor()
    {
        float progress = MathHelper.Clamp(timeSystem.PhaseProgress, 0f, 1f);

        if (timeSystem.CurrentDayTime == DayTime.Day)
        {
            // 0.00–0.15: dawn -> day
            if (progress < TransitionDuration)
            {
                float t = progress / TransitionDuration;
                return Color.Lerp(dawnColor, dayColor, t);
            }

            // 0.15–0.85: day
            if (progress < 1f - TransitionDuration)
                return dayColor;

            // 0.85–1.00: day -> sunset
            float tSunset = (progress - (1f - TransitionDuration)) / TransitionDuration;
            return Color.Lerp(dayColor, sunsetColor, tSunset);
        }

        // 0.00–0.15: sunset -> dusk
        if (progress < TransitionDuration)
        {
            float t = progress / TransitionDuration;
            return Color.Lerp(sunsetColor, duskColor, t);
        }

        // 0.15–0.30: dusk -> night
        if (progress < TransitionDuration * 2f)
        {
            float t = (progress - TransitionDuration) / TransitionDuration;
            return Color.Lerp(duskColor, nightColor, t);
        }

        // 0.30–0.85: night
        if (progress < 1f - TransitionDuration)
            return nightColor;

        // 0.85–1.00: night -> dawn
        float tDawn = (progress - (1f - TransitionDuration)) / TransitionDuration;

        return Color.Lerp(nightColor, dawnColor, tDawn);
    }
}