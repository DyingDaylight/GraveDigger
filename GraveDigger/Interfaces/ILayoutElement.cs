using GraveDigger.GUI.Elements;
using Microsoft.Xna.Framework;

namespace Interfaces;

public interface ILayoutElement
{
    public Vector2 VisibleSize { get; }
    public void SetPosition(int x, int y);

}