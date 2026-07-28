using GraveDigger.Core;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.Props;

public class Lamppost : Decoration, ILightSource
{
    private Texture2D light;
    private readonly Color lightColor = new(255, 160, 60);
    
    public Vector2 LightAnchorUV { get; set; } = new(0.77f, 0.30f);
    
    public Lamppost(string name) : base(name)
    {
    }

    public override void Start()
    {
        base.Start();
        light = SpriteManager.GetSprite("light").Texture;
    }
    
    public void DrawLight(SpriteBatch spriteBatch)
    {
        if (!IsUnlocked)
            return;
        
        Vector2 offset = new(
            LightAnchorUV.X * light.Width,
            LightAnchorUV.Y * light.Height);
        offset *= Transform.Scale;

        if (SpriteEffect == SpriteEffects.FlipHorizontally)
            offset.X = -offset.X;

        Vector2 lightPosition = Transform.Position - new Vector2(light.Width, light.Height) * 0.5f
            + offset;
        
        spriteBatch.Draw(light, lightPosition, lightColor);
    }
}