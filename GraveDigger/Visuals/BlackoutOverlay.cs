using System;
using GraveDigger.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GraveDigger.Visuals;

public class BlackoutOverlay : IDrawable
{
    private enum BlackoutState
    {
        Hidden,
        FadingIn,
        Hold,
        FadingOut
    }
    
    private const float FadeDuration = 0.25f;
    private const float HoldDuration = 0.30f;
    
    private BlackoutState state;
    private float elapsedTime;
    private Action onCovered;
    
    public float Opacity { get; private set; }
    public bool IsRunning => state != BlackoutState.Hidden;

    private readonly Color color = Color.Black;
    
    private readonly Texture2D overlay;
    private readonly Rectangle bounds;

    public BlackoutOverlay(Vector2 screenSize)
    {
        overlay = SpriteManager.GetSprite("pixel").Texture;
        bounds = new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y);
    }
    
    public void Update(GameTime gameTime)
    {
        if (state == BlackoutState.Hidden)
            return;
        
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        elapsedTime += deltaTime;
        
        switch (state)
        {
            case BlackoutState.FadingIn:
                Opacity = MathHelper.Clamp(elapsedTime / FadeDuration,0f, 1f);

                if (elapsedTime >= FadeDuration)
                {
                    Opacity = 1f;
                    elapsedTime = 0f;

                    state = BlackoutState.Hold;
                }
                break;
            
            case BlackoutState.Hold:
                Opacity = 1f;

                if (elapsedTime >= HoldDuration)
                {
                    elapsedTime = 0f;
                    state = BlackoutState.FadingOut;
                    
                    onCovered?.Invoke();
                    onCovered = null;
                }
                break;
            
            case BlackoutState.FadingOut:
                Opacity = 1f - MathHelper.Clamp(elapsedTime / FadeDuration,0f,1f);

                if (elapsedTime >= FadeDuration)
                {
                    Opacity = 0f;
                    elapsedTime = 0f;
                    state = BlackoutState.Hidden;
                }
                break;
        }
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (state == BlackoutState.Hidden)
            return;
        
        spriteBatch.Draw(overlay, bounds, color * Opacity);
    }

    public void Run(Action onCovered)
    {
        if (state != BlackoutState.Hidden)
            return;

        this.onCovered = onCovered;
        elapsedTime = 0f;
        state = BlackoutState.FadingIn;
    }
}