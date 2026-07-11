using GraveDigger.Utils;
using Microsoft.Xna.Framework;

namespace GraveDigger;

public class GameContext
{
    public CoordinatesConverter CoordinatesConverter { get; }
    public RandomService RandomService { get; }
    
    public Vector2 ScreenSize { get; }
    
    public GameContext(Camera camera, Vector2 screenSize, RandomService randomService)
    {
        CoordinatesConverter = new CoordinatesConverter(camera);
        ScreenSize = screenSize;
        RandomService = randomService;
    }
    
}