using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger;

public class Camera : IUpdatable
{
    public Vector2 Position; // Top Left Corner
    public Matrix TransformMatrix;
    
    private readonly Viewport _viewport;
    
    private Vector2 targetPosition;
    
    public Camera(Viewport viewport)
    {
        _viewport = viewport;
    }

    public void Start()
    {
        
    }

    public void Update(GameTime gameTime)
    {
        var offset = new Vector2(_viewport.Width / 2f, _viewport.Height / 2f);
        Position = targetPosition - offset;
        
        if (targetPosition.X - offset.X < 0)
        { 
            Position.X = 0;
        } 
        else if (targetPosition.X + offset.X > Game1.WorldSize.X)
        {
            Position.X = Game1.WorldSize.X - _viewport.Width;
        }
        
        if (targetPosition.Y - offset.Y < 0)
        {
            Position.Y = 0;
        } 
        else if (targetPosition.Y + offset.Y > Game1.WorldSize.Y)
        {
            Position.Y = Game1.WorldSize.Y - _viewport.Height;
        } 
        
        TransformMatrix = Matrix.CreateTranslation(new Vector3(-Position, 0));
    }

    public void SetTarget(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
}