using System;
using GraveDigger.Core;
using GraveDigger.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraveDigger.Characters;

public class Player : Animation
{
    private const float MovementSpeed = 300f;
    private const int AnimationFps = 4;

    public Collider Collider { get; }

    private Vector2 previousPosition;
    
    private Vector2 worldSize = Vector2.Zero;
    private int previousAnimationRow = -1;
    private bool isColliding = false;
    
    public int Hunger { get; private set; }
    public int MaxHunger { get; } = 100;
    public bool IsStarving => Hunger >= MaxHunger;
    
    public event Action<int, int, int> HungerChanged;
    
    public Player() : base("digger")
    {
        Collider = new Collider(this);
        Collider.IsTrigger = false;
        ShadowOffsetY = 0;
    }
    
    public override void Start()
    {
        base.Start();

        CastShadow = true;
        ShadowOffsetY = 0f;
        
        previousPosition = Transform.Position;
        Transform.Scale = new Vector2(0.21f, 0.21f);
        
        CurrentRow = 1; 
        Stop();
        
        Collider.Start();
    }
    
    public override void Update(GameTime gameTime)
    {
        // Collisions are not working
        if (isColliding)
        {
            Transform.Position = previousPosition;
            isColliding = false;
        }
        
        previousPosition = Transform.Position;
        
        float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
        
        UpdateMovement(dt);
        UpdateSortingOrder();
        
        Collider.Update(gameTime);
        
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        Collider.Draw(spriteBatch);
    }
    
    public void OnTriggerEnter(Collider self, Collider other)
    {
    }
    
    public void OnCollisionEnter(Collider self, Collider other)
    {
    }
    
    public void DecreaseHunger(int amount)
    {
        if (amount <= 0)
            return;

        Hunger = Math.Clamp(Hunger - amount, 0, MaxHunger);
        HungerChanged?.Invoke(Hunger, 0, MaxHunger);
    }
    
    public void IncreaseHunger(int amount)
    {
        if (amount <= 0)
            return;

        Hunger = Math.Clamp(Hunger + amount, 0, MaxHunger);
        HungerChanged?.Invoke(Hunger, 0, MaxHunger);
    }
    
    public void SetWorldSize(Vector2 gameContextWorldSize)
    {
        worldSize = gameContextWorldSize;
    }
    
    private void UpdateSortingOrder()
    {
        SortingOrder = SortingUtility.CalculateByY(Bottom);
    }
    
    private void UpdateMovement(float dt)
    {
        KeyboardState keyboard = Keyboard.GetState();
        Vector2 direction = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.W))
            direction.Y -= 1;
        
        if (keyboard.IsKeyDown(Keys.S))
            direction.Y += 1;

        if (keyboard.IsKeyDown(Keys.A))
            direction.X -= 1;
        
        if (keyboard.IsKeyDown(Keys.D))
            direction.X += 1;
    
        if (direction != Vector2.Zero)
        {
            direction.Normalize();

            UpdateMovementAnimation(direction);
            
            Systems.AudioManager.Instance.PlaySFX("steps", loop: true);

            if (CurrentRow != previousAnimationRow)
            {
                Reset();
                previousAnimationRow = CurrentRow;
            }

            Play(AnimationFps);
            Transform.Position += direction * MovementSpeed * dt;
        }
        else
        {
            Systems.AudioManager.Instance.PauseSFX("steps");
            
            Stop();
            previousAnimationRow = -1;
        }
    
        ClampToWorld();
    }

    private void UpdateMovementAnimation(Vector2 direction)
    {
        if (MathF.Abs(direction.X) > MathF.Abs(direction.Y))
        {
            CurrentRow = direction.X < 0 ? 2 : 3;
        }
        else
        {
            CurrentRow = direction.Y < 0 ? 0 : 1;
        }
    }

    private void ClampToWorld()
    {
        if (worldSize == Vector2.Zero)
            return;
        
        Vector2 position = Transform.Position;

        float scaledOriginX = Origin.X * Transform.Scale.X;
        float scaledOriginY = Origin.Y * Transform.Scale.Y;
        
        position.X = MathHelper.Clamp(position.X, scaledOriginX, worldSize.X - Width + scaledOriginX);
        position.Y = MathHelper.Clamp(position.Y, scaledOriginY, worldSize.Y - Height + scaledOriginY);
        
        Transform.Position = position;
    }
}