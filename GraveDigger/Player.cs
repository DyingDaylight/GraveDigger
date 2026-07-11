using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraveDigger;

public class Player : Animation
{
    private const float MovementSpeed = 300;
    private const int AnimationFps = 4;
    
    public Collider Collider;

    public Vector2 previousPosition;
    
    private int previousRow = -1;
    private bool isColliding = false;
    
    public Player() : base("digger")
    {
        Collider = new Collider();
        Collider.Parent = this;  
        Collider.isTrigger = false;
    }
    
    public override void Start()
    {
        base.Start();
        
        Transform.Position = new Vector2(Game1.ScreenSize.X * 0.5f, Game1.ScreenSize.Y * 0.5f);
        previousPosition = Transform.Position;
        
        Transform.Scale = new Vector2(0.15f, 0.15f);
        
        CurrentRow = 1; 
        Stop();
        
        Collider.Start();
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

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

    private void UpdateSortingOrder()
    {
        float depth = Bottom / Game1.WorldSize.Y;
        SortingOrder = 1f - MathHelper.Clamp(depth, 0f, 1f);
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

            if (CurrentRow != previousRow)
            {
                Reset();
                previousRow = CurrentRow;
            }

            Play(AnimationFps);
            Transform.Position += direction * MovementSpeed * dt;
        }
        else
        {
            Stop();
            previousRow = -1;
        }
    
        ClampToScreen();
    }
    
    private void ClampToScreen()
    {
        Vector2 position = Transform.Position;

        float scaledOriginX = Origin.X * Transform.Scale.X;
        float scaledOriginY = Origin.Y * Transform.Scale.Y;
        
        position.X = MathHelper.Clamp(position.X, scaledOriginX, Game1.WorldSize.X - Width + scaledOriginX);
        position.Y = MathHelper.Clamp(position.Y, scaledOriginY, Game1.WorldSize.Y - Height + scaledOriginY);
        
        Transform.Position = position;
    }
    
    public void OnTriggerEnter(Collider self, Collider other)
    {
        Console.WriteLine("OnTriggerEnter " + self.Parent + " with " + other.Parent);
    }
    
    public void OnCollisionEnter(Collider self, Collider other)
    {
        Console.WriteLine("OnCollisionEnter " + self.Parent + " with " + other.Parent);
        isColliding = true;
    }
}