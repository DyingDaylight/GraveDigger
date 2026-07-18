using System;
using GraveDigger.Core;
using GraveDigger.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraveDigger;

public class Player : Animation
{
    private const float MovementSpeed = 300;
    private const int AnimationFps = 4;
    
    public Collider Collider;

    private GameContext gameContext;
    private Vector2 previousPosition;
    
    private int previousAnimationRow = -1;
    private bool isColliding = false;
    
    public int Hunger { get; private set; }
    public int MaxHunger { get; } = 100;
    public bool IsStarving => Hunger >= MaxHunger;
    
    public event Action<int, int, int> HungerChanged;
    
    public Player(GameContext gameContext) : base("digger")
    {
        this.gameContext = gameContext;
        Collider = new Collider(this);
        Collider.IsTrigger = false;
    }
    
    public override void Start()
    {
        base.Start();

        CastShadow = true;
        
        Transform.Position = new Vector2(gameContext.ScreenSize.X * 0.5f, gameContext.ScreenSize.Y * 0.5f);
        previousPosition = Transform.Position;
        
        Transform.Scale = new Vector2(0.21f, 0.21f);
        
        CurrentRow = 1; 
        Stop();
        
        Collider.Start();
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

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
    
    private void UpdateSortingOrder()
    {
        SortingOrder = SortingUtility.CalculateByY(Bottom);
    }
    
    private void UpdateMovement(float dt)
    {
        KeyboardState keyboard = Keyboard.GetState();
        Vector2 direction = Vector2.Zero;
        bool isMoving = false;

        if (keyboard.IsKeyDown(Keys.W))
        {
            direction.Y -= 1;
            CurrentRow = 0; 
            isMoving = true;
        }
        else if (keyboard.IsKeyDown(Keys.S))
        {
            direction.Y += 1;
            CurrentRow = 1; 
            isMoving = true;
        }

        if (keyboard.IsKeyDown(Keys.A))
        {
            direction.X -= 1;
            CurrentRow = 2; 
            isMoving = true;
        }
        else if (keyboard.IsKeyDown(Keys.D))
        {
            direction.X += 1;
            CurrentRow = 3; 
            isMoving = true;
        }
    
        if (isMoving)
        {
            if (direction != Vector2.Zero)
                direction.Normalize();
            
            GraveDigger.Systems.AudioManager.Instance.PlaySFX("steps", loop: true);

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
    
    private void ClampToWorld()
    {
        Vector2 position = Transform.Position;

        float scaledOriginX = Origin.X * Transform.Scale.X;
        float scaledOriginY = Origin.Y * Transform.Scale.Y;
        
        position.X = MathHelper.Clamp(position.X, scaledOriginX, gameContext.WorldSize.X - Width + scaledOriginX);
        position.Y = MathHelper.Clamp(position.Y, scaledOriginY, gameContext.WorldSize.Y - Height + scaledOriginY);
        
        Transform.Position = position;
    }
}