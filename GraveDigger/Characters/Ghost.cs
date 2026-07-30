using System;
using GraveDigger.Core;
using GraveDigger.Utils;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.Characters;

public class Ghost : Animation, IReputationContributor
{
    private const int ReputationModifier = -7;
    private const float StopDistance = 30f;
    private const float MoveSpeed = 120f;
    private const int AnimationFps = 5;

    private const float BaseOpacity = 0.7f;
    private const float OpacityAmplitude = 0.08f;
    private const float OpacitySpeed = 3f;
    
    private float totalTime = 0;
    
    public Vector2 TargetPosition { get; set; }
    
    public Ghost() : base("ghost")
    {
        CastShadow = true;
        ShadowOffsetY = 0;
        ShadowOpacity = 0.1f;
        
        Opacity = BaseOpacity;
    }

    public override void Update(GameTime gameTime)
    {
        float deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

        UpdateOpacity(deltaTime);
        UpdateMovement(deltaTime);
        UpdateSortingOrder();
        
        Play(AnimationFps);

        base.Update(gameTime);
    }

    public int GetReputationValue()
    {
        return ReputationModifier;
    }
    
    private void UpdateOpacity(float deltaTime)
    {
        totalTime += deltaTime;
        Opacity = BaseOpacity + OpacityAmplitude * MathF.Sin(totalTime * OpacitySpeed);
    }

    private void UpdateMovement(float deltaTime)
    {
        Vector2 direction = TargetPosition - Transform.Position;
        
        float distance = direction.Length();

        if (distance <= StopDistance)
        {
            CurrentRow = 3;
            SpriteEffect = SpriteEffects.None;
            return;
        }
        
        direction.Normalize();
        
        float moveDistance = MoveSpeed * deltaTime;

        Transform.Position += direction * 
                              MathF.Min(moveDistance, distance - StopDistance);

        UpdateMovementAnimation(direction);
    }

    private void UpdateMovementAnimation(Vector2 direction)
    {
        if (Math.Abs(direction.X) > Math.Abs(direction.Y))
        {
            CurrentRow = 2;
            SpriteEffect = direction.X < 0
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;
        }
        else
        {
            CurrentRow = direction.Y > 0 ? 1 : 0;
            SpriteEffect = SpriteEffects.None;
        }
    }
    
    private void UpdateSortingOrder()
    {
        SortingOrder = SortingUtility.CalculateByY(Bottom);
    }
}