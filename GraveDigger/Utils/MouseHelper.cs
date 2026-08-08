using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GraveDigger.Utils;

// Provides mouse input in virtual game coordinates.
// The game is rendered at a fixed virtual resolution and then scaled and
// centered to fit the actual screen. This helper converts the physical mouse
// position back to the corresponding position within the virtual resolution,
// accounting for both scaling and letterboxing offsets.
public static class MouseHelper
{
    private static Rectangle destinationRectangle;
    private static int virtualWidth;
    private static int virtualHeight;

    public static void Configure(Rectangle destination, int width, int height)
    {
        destinationRectangle = destination;
        virtualWidth = width;
        virtualHeight = height;
    }

    public static MouseState GetState()
    {
        MouseState state = Mouse.GetState();

        Point virtualPosition = ScreenToVirtual(new Vector2(state.X, state.Y)).ToPoint();

        return new MouseState(
            virtualPosition.X,
            virtualPosition.Y,
            state.ScrollWheelValue,
            state.LeftButton,
            state.MiddleButton,
            state.RightButton,
            state.XButton1,
            state.XButton2
        );
    }

    private static Vector2 ScreenToVirtual(Vector2 screenPosition)
    {
        // Convert the mouse position from physical screen coordinates to virtual
        // game coordinates. First, subtract the destination offset to get the
        // position relative to the rendered game area, then undo the scaling.
        float x =
            (screenPosition.X - destinationRectangle.X)
            * virtualWidth
            / destinationRectangle.Width;

        float y =
            (screenPosition.Y - destinationRectangle.Y)
            * virtualHeight
            / destinationRectangle.Height;

        return new Vector2(x, y);
    }
}