using Microsoft.Xna.Framework;

namespace Interfaces;

public interface IUpdatable
{
    public void Start();
    public void Update(GameTime gameTime);
}