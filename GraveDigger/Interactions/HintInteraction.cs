namespace GraveDigger.Interactions;

public class HintInteraction : Interaction
{
    public HintInteraction(IInteractionOwner interactionOwner) : base(interactionOwner)
    {
        Hint = "Use blueprints";
    }

    public override void Interact()
    {
        // Do nothing, just show hint
    }
}