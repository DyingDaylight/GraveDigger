using System;
using GraveDigger.Core;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.Props;

public class Lamppost : Decoration, ILightSource
{
    private Texture2D light;
    private readonly Color lightColor = new(255, 160, 60);
    
    public Vector2 LightOffset { get; set; } = new(10, -30);
    
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

        spriteBatch.Draw(light,
            new Vector2(Transform.Position.X - light.Bounds.Width * 0.5f + LightOffset.X,
                Transform.Position.Y - light.Bounds.Height * 0.5f + LightOffset.Y),
             lightColor);
    }
}