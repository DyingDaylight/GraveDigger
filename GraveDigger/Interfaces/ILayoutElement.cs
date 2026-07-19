using Microsoft.Xna.Framework;

namespace Interfaces;

public interface ILayoutElement
{
    Vector2 Size { get; }
    bool Visible { get; }
    void SetPosition(int x, int y);
}