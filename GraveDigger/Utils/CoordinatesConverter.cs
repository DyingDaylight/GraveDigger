using GraveDigger.Core;
using Microsoft.Xna.Framework;

namespace GraveDigger.Utils;

public class CoordinatesConverter
{
    private readonly Camera camera;
    
    public CoordinatesConverter(Camera camera)
    {
        this.camera = camera;
    }

    public Vector2 WorldToScreen(Vector2 worldPosition)
    {
        return Vector2.Transform(worldPosition, camera.TransformMatrix);
    }

    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {   
        Matrix inverseViewMatrix = Matrix.Invert(camera.TransformMatrix);
        return Vector2.Transform(screenPosition, inverseViewMatrix);
    }
}