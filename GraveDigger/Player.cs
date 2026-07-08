using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraveDigger;

public class Player : Animation
{
    private const float MovementSpeed = 300;
    
    public Collider Collider;

    public Vector2 previousPosition;
    
    private bool isColliding = false;
    
    public Player() : base("digger")
    {
        Collider = SceneManager.Create<Collider>();
        Collider.Parent = this;  
        Collider.isTrigger = false;
    }
    
    public override void Start()
    {
        base.Start();
        
        Transform.Position = new Vector2(Game1.ScreenSize.X * 0.5f, Game1.ScreenSize.Y * 0.5f);
        previousPosition = Transform.Position;
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
    }
    
    private void UpdateSortingOrder()
    {
        // Update layer depth every frame because the player can move vertically.
        float depth = Bottom / Game1.ScreenSize.Y;
        SortingOrder = 1f - MathHelper.Clamp(depth, 0f, 1f);
    }
    
    private void UpdateMovement(float dt)
    {
        KeyboardState keyboard = Keyboard.GetState();

        Vector2 direction = Vector2.Zero;
        if (keyboard.IsKeyDown(Keys.D))
        {
            SpriteEffect = SpriteEffects.FlipHorizontally;
            direction.X += 1;
        }
        
        if (keyboard.IsKeyDown(Keys.A))
        {
            SpriteEffect = SpriteEffects.None;   
            direction.X -= 1;
        }
        
        if (keyboard.IsKeyDown(Keys.W))
            direction.Y -= 1;
        
        if (keyboard.IsKeyDown(Keys.S)) 
            direction.Y += 1;
        
        if (direction != Vector2.Zero)
            direction.Normalize();
        
        Transform.Position += direction * MovementSpeed * dt;
        
        ClampToScreen();
    }
    
    private void ClampToScreen()
    {
        // Clamp the sprite origin so the whole player remains inside the screen.
        Vector2 position = Transform.Position;

        float scaledOriginX = Origin.X * Transform.Scale.X;
        float scaledOriginY = Origin.Y * Transform.Scale.Y;
        
        position.X = MathHelper.Clamp(position.X, scaledOriginX, Game1.ScreenSize.X - Width + scaledOriginX);
        position.Y = MathHelper.Clamp(position.Y, scaledOriginY, Game1.ScreenSize.Y - Height + scaledOriginY);
        
        Transform.Position = position;
    }
    
    public void OnTriggerEnter(Collider self, Collider other)
    {
        Console.WriteLine("OnTriggerEnter " + self.Parent + " with " + other.Parent);
        
    }

    public void OnTriggerStay(Collider self, Collider other)
    {
    }

    public void OnCollisionEnter(Collider self, Collider other)
    {
        Console.WriteLine("OnCollisionEnter " + self.Parent + " with " + other.Parent);
        
        //SceneManager.Remove(other.Parent);
        //SceneManager.Remove(other);
        
        isColliding = true;
    }
}