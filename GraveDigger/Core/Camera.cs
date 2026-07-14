using System;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.Core;

public class Camera : IUpdatable
{
    private readonly Viewport viewport;
    private readonly Vector2 worldSize;

    private Vector2 position; // Top Left Corner
    private Vector2 targetWorldPosition;

    public Matrix TransformMatrix { get; private set; }

    public Camera(Viewport viewport, Vector2 worldSize)
    {
        this.viewport = viewport;
        this.worldSize = worldSize;
    }

    public void Start()
    {
    }

    public void Update(GameTime gameTime)
    {
        var offset = new Vector2(viewport.Width * 0.5f, viewport.Height * 0.5f);
        position = targetWorldPosition - offset;
        
        position.X = MathHelper.Clamp(position.X, 0, worldSize.X - viewport.Width);
        position.Y = MathHelper.Clamp(position.Y, 0, worldSize.Y - viewport.Height);
        
        position.X = MathF.Round(position.X);
        position.Y = MathF.Round(position.Y);
        
        TransformMatrix = Matrix.CreateTranslation(new Vector3(-position, 0));
    }

    public void SetTarget(Vector2 targetPosition)
    {
        targetWorldPosition = targetPosition;
    }
}