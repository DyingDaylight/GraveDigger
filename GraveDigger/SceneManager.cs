using System.Collections.Generic;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GraveDigger;

public class SceneManager : IUpdatable, IDrawable
{
    static List<IUpdatable> updatables = new List<IUpdatable>();
    static List<IDrawable> drawables = new List<IDrawable>();
    public static List<Collider> colliders = new List<Collider>();

    private static SceneManager instance = null;

    public static SceneManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SceneManager();
            }
            
            return instance;
        }
    }
    
    public static T Create<T>() where T : IUpdatable, new()
    {
        T newObject = new T();
        
        updatables.Add(newObject);

        if (newObject is IDrawable drawable)
            drawables.Add(drawable);
        
        if (newObject is Collider collider)
            colliders.Add(collider);
        
        return newObject;
    }

    public static bool Remove<T>(T obj) where T : IUpdatable
    {
        // TODO: add validation
        updatables.Remove(obj);
        
        if (obj is IDrawable drawable)
        {
            drawables.Remove(drawable);
        }
        
        if (obj is Collider collider)
        {
            colliders.Remove(collider);
        }
        
        return true;
    }

    public void Start()
    {
        for (int i = 0; i < updatables.Count; i++)
        {
            updatables[i].Start();
        }
    }

    public void Update(GameTime gameTime)
    {
        for (int i = 0; i < updatables.Count; i++)
        {
            updatables[i].Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < drawables.Count; i++)
        {
            drawables[i].Draw(spriteBatch);
        }
    }
}