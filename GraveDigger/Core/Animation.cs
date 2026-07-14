using Microsoft.Xna.Framework;

namespace GraveDigger.Core;

public class Animation : Sprite
{
    private double totalTime;
    private int fps = 60;

    protected int currentColumn = 0;
    protected int currentRow = 0;

    public bool IsLooping { get; set; } = true;
    private bool isAnimating = false;

    public int CurrentRow
    {
        get => currentRow;
        set => currentRow = MathHelper.Clamp(value, 0, SpriteSheet.Rows - 1);
    }
    
    public Animation(string name) : base(name)
    {
    }
    
    public override void Update(GameTime gameTime)
    {
        if (ShouldMoveToNextFrame(gameTime))
        {
            totalTime = 0.0f;
            currentColumn++;
            if (currentColumn >= SpriteSheet.Columns)
            {
                if (IsLooping)
                {
                    currentColumn = 0;
                }
                else
                {
                    currentColumn = SpriteSheet.Columns - 1;
                }
            }
        }
        
        SourceRectangle = SpriteSheet[currentColumn, currentRow];
        base.Update(gameTime);
    }
    
    public void Play(int fps = 60)
    {
        isAnimating = true;    
        this.fps = fps > 0 ? fps : 1;
    }
    
    public void Stop()
    {
        isAnimating = false;
        currentColumn = 0;
        totalTime = 0;
    }

    public void Reset()
    {
        currentColumn = 0;
        totalTime = 0;
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