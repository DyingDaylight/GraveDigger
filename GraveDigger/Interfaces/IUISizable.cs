using GraveDigger.GUI.Elements;
using Microsoft.Xna.Framework;

namespace Interfaces;

public interface IUISizable
{
    public Vector2 VisibleSize { get; }
    public bool IsSpacer { get; }
    public void SetPosition(int x, int y);

}