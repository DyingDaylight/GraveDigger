using Microsoft.Xna.Framework;

namespace GraveDigger;

public class Animation : Sprite
{
    private double totalTime;
    private int fps = 60;

    protected int x = 0;
    protected int y = 0;

    private bool isLooping = true;
    private bool isAnimating = false;

    public int CurrentRow
    {
        get => y;
        set => y = MathHelper.Clamp(value, 0, SpriteSheet.Rows - 1);
    }
    
    public Animation(string name) : base(name)
    {
    }

    public override void Start()
    {
        base.Start();
    }

    public void Stop()
    {
        isAnimating = false;
        x = 0; 
    }

    public void Play(int fps = 60)
    {
        isAnimating = true;    
        this.fps = fps;
    }

    public void Reset()
    {
        x = 0;
        totalTime = 0;
    }
    
    public override void Update(GameTime gameTime)
    {
        if (ShouldMoveToNextFrame(gameTime))
        {
            totalTime = 0.0f;
            x++;
            if (x >= SpriteSheet.Columns)
            {
                if (isLooping)
                {
                    x = 0;
                }
                else
                {
                    x = SpriteSheet.Columns - 1;
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