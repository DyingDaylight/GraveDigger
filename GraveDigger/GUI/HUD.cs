using System;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GUI;

public class HUD : IUpdatable, IDrawable
{
    public void Start()
    {
        
    }

    public void Update(GameTime gameTime)
    {
       
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        
    }

    public void UpdateReputation(int value)
    {
        // TODO: draw interface on screen
        Console.WriteLine("Reputation: " + value);
    }
}