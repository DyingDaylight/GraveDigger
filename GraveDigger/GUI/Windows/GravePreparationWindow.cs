using System;
using GraveDigger.GraveSites;
using GraveDigger.GUI.Elements;
using GraveDigger.GUI.Layouts;
using GUI.Windows;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Windows;

public class GravePreparationWindow : Window
{
    private readonly Label infoLabel;
    
    private readonly Button prepareButton;
    private readonly Button closeButton;
    
    private readonly HorizontalLayout buttonsLayout;
    private readonly VerticalLayout infoLayout;
    
    private GraveSite? graveSite;
    
    public event Action<GraveSite> PrepareButtonPressed;
    public event Action OnCloseButton;
    
    public GravePreparationWindow(Rectangle parentBounds) : base(parentBounds)
    {
        infoLabel = CreateElement<Label>();
        infoLabel.Text =
            "Prepare this burial plot?\n" +
            "\n" +
            "Digging the grave will prepare this plot for a future burial.\n" +
            "Until someone is buried here, it will negatively affect your\n" +
            "reputation.";
        
        int buttonWidth = 270;
        int buttonHeight = 80;
        
        prepareButton = CreateButton("Prepare", buttonWidth, buttonHeight, HandlePrepareClick);
        closeButton = CreateButton("Close", buttonWidth, buttonHeight, HandleCloseButton);
        
        buttonsLayout = new HorizontalLayout(Bounds);
        buttonsLayout.HorizontalPadding = 20;
        buttonsLayout.SetPosition(Bounds.X, Bounds.Bottom - 190);

        buttonsLayout.AddElement(prepareButton);
        buttonsLayout.AddElement(closeButton);
        
        Rectangle contentBounds = new Rectangle(Bounds.X + 40, Bounds.Y, Bounds.Width - 70, Bounds.Height - 120);
        infoLayout = new VerticalLayout(contentBounds);
        infoLayout.VerticalPadding = 40;
       
        infoLayout.AddElement(infoLabel);
        
        RefreshLayout();
    }
    
    public void SetData(GraveSite graveSite)
    {
        this.graveSite = graveSite;
    }
    
    private void HandlePrepareClick()
    {
        PrepareButtonPressed?.Invoke(graveSite);
        OnCloseButton?.Invoke();
    }

    private void HandleCloseButton()
    {
        OnCloseButton?.Invoke();
    }
    
    protected override void RefreshLayout()
    {
        buttonsLayout.UpdateLayout();
        infoLayout.UpdateLayout();
    }
}