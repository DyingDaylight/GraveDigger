using Microsoft.Xna.Framework;

namespace GraveDigger.Interactions;

public interface IInteractionOwner
{
    Rectangle InteractionArea { get; }
    
    void SetHighlighted(bool highlighted);
}