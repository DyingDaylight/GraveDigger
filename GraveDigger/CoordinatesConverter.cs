using System.Runtime.InteropServices.ObjectiveC;
using Microsoft.Xna.Framework;

namespace GraveDigger;

public class CoordinatesConverter
{
    public Camera Camera { get; }
    
    public CoordinatesConverter(Camera camera)
    {
        Camera = camera;
    }

    public Vector2 WorldToScreen(Vector2 worldPosition)
    {
        return Vector2.Transform(worldPosition, Camera.TransformMatrix);
    }

    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {   
        Matrix inverseViewMatrix = Matrix.Invert(Camera.TransformMatrix);
        return Vector2.Transform(screenPosition, inverseViewMatrix);
    }
}