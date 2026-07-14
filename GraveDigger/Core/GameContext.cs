using GraveDigger.Utils;
using Microsoft.Xna.Framework;

namespace GraveDigger.Core;

public class GameContext
{
    public Vector2 ScreenSize { get; }
    
    public CoordinatesConverter CoordinatesConverter { get; }
    public RandomService RandomService { get; }
    
    public GameContext(Camera camera, Vector2 screenSize, RandomService randomService)
    {
        CoordinatesConverter = new CoordinatesConverter(camera);
        ScreenSize = screenSize;
        RandomService = randomService;
    }
    
}