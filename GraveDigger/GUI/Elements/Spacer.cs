using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Elements;

public class Spacer : ILayoutElement, ISpacer
{
    public Vector2 VisibleSize { get; }

    // A spacer has no visual representation or position.
    public void SetPosition(int x, int y)
    {
    }
}