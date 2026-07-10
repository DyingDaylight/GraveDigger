using Microsoft.Xna.Framework;

namespace GraveDigger;

public class GameContext
{
    public CoordinatesConverter CoordinatesConverter { get; }

    public Vector2 ScreenSize { get; }
    
    public GameContext(Camera camera, Vector2 screenSize)
    {
        CoordinatesConverter = new CoordinatesConverter(camera);
        ScreenSize = screenSize;
    }
    
}