using GraveDigger.Utils;
using Microsoft.Xna.Framework;

namespace GraveDigger.Core;

public class GameContext
{
    public Vector2 ScreenSize { get; }
    public Vector2 WorldSize { get; }
    
    public CoordinatesConverter CoordinatesConverter { get; }
    public RandomService RandomService { get; }
    
    public GameContext(Camera camera, Vector2 screenSize, Vector2 worldSize, RandomService randomService)
    {
        CoordinatesConverter = new CoordinatesConverter(camera);
        ScreenSize = screenSize;
        WorldSize = worldSize;
        RandomService = randomService;
    }
    
}