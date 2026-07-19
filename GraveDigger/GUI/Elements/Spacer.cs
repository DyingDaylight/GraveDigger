using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Elements;

public class Spacer : ILayoutElement, ISpacer
{
    public Vector2 Size { get; }
    public bool Visible { get; } = true;

    // A spacer occupies space in a layout but has no visual representation.
    public void SetPosition(int x, int y)
    {
    }
}