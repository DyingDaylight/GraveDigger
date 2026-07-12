using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Elements;

public class Spacer : IUISizable
{
    public Vector2 VisibleSize { get; }
    public bool IsSpacer => true;

    public void SetPosition(int x, int y)
    {
        throw new System.NotImplementedException();
    }
}