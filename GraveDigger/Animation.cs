using Microsoft.Xna.Framework;

namespace GraveDigger;

public class Animation : Sprite
{
    private double totalTime;
    private int fps = 60;

    private int x = 0;
    private int y = 0;

    private bool isLooping = true;
    private bool isAnimating = false;
    
    public Animation(string name) : base(name)
    {
    }

    public override void Start()
    {
        base.Start();
    }

    public void Play(int fps = 60)
    {
        isAnimating = true;    
        this.fps = fps;
        
        Reset();
    }

    public void Reset()
    {
        x = 0;
        y = 0;
        totalTime = 0;
    }
    
    public override void Update(GameTime gameTime)
    {
        if (ShouldMoveToNextFrame(gameTime))
        {
            totalTime = 0.0f;
            x++;
            if (x == SpriteSheet.Columns)
            {
                x = 0;
                y++;
                if (y == SpriteSheet.Rows)
                {
                    if (isLooping)
                    {
                        x = 0;
                        y = 0;
                    }
                    else
                    {
                        x = SpriteSheet.Columns - 1;
                        y = SpriteSheet.Rows - 1;
                    }
                }
            }
        }
        
        sourceRectangle = SpriteSheet[x, y];
        base.Update(gameTime);
    }

    private bool ShouldMoveToNextFrame(GameTime gameTime)
    {
        if (!isAnimating) return false;
        
        double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
        totalTime += deltaTime;
        
        if (totalTime >= 1.0f / fps)
            return true;
        
        return false;
    }
}