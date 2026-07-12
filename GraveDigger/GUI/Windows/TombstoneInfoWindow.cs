using System;
using GraveDigger.GUI.Elements;
using GraveDigger.GUI.Layouts;
using GraveDigger.Props;
using GUI.Windows;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Windows;

public class TombstoneInfoWindow : Window
{
    private readonly Label nameLabel;
    private readonly Label yearLabel;
    private readonly Label wealthLabel;
    private readonly Label natureLabel;
    private readonly Label stateLabel;

    private readonly Button digButton;
    private readonly Button repairButton;
    private readonly Button closeButton;
    
    private readonly HorizontalLayout buttonsLayout;
    private readonly VerticalLayout infoLayout;
    
    private Tombstone tombstone;
    
    public event Action<Tombstone> OnDigButton;
    public event Action<Tombstone> OnRepairButton;
    public event Action OnCloseButton;
    
    public TombstoneInfoWindow()
    {
        nameLabel = CreateElement<Label>();
        yearLabel = CreateElement<Label>();
        wealthLabel = CreateElement<Label>();
        natureLabel = CreateElement<Label>();
        stateLabel = CreateElement<Label>();
        
        digButton = CreateElement<Button>();
        digButton.SetText("Dig");
        digButton.OnClick += HandleDigClick;
        
        repairButton = CreateElement<Button>();
        repairButton.SetText("Repair");
        repairButton.OnClick += HandleRepairClick;
        
        closeButton = CreateElement<Button>();
        closeButton.SetText("Close");
        closeButton.OnClick += HandleCloseButton;
        
        buttonsLayout = new HorizontalLayout(Bounds);
        buttonsLayout.HorizontalPadding = 20;
        buttonsLayout.SetPosition(Bounds.X, Bounds.Bottom - 120);
        
        buttonsLayout.AddElement(digButton);
        buttonsLayout.AddElement(repairButton);
        buttonsLayout.AddElement(closeButton);
        buttonsLayout.UpdateLayout();
        
        Rectangle contentBounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height - 120);
        infoLayout = new VerticalLayout(contentBounds);
        infoLayout.VerticalPadding = 40;
       
        infoLayout.AddElement(nameLabel);
        infoLayout.AddElement(yearLabel);
        infoLayout.AddElement(wealthLabel);
        infoLayout.AddElement(natureLabel);
        infoLayout.AddElement(stateLabel);
        infoLayout.UpdateLayout();
    }

    public void SetData(Tombstone tombstone)
    {
        this.tombstone = tombstone;
        RefreshContent();
    }

    public void Refresh()
    {
        if (tombstone == null)
            return;
        
        RefreshContent();
    }

    private void RefreshContent()
    {
        nameLabel.Text = tombstone.Data.Name;
        yearLabel.Text = tombstone.Data.Years;
        wealthLabel.Text = $"Wealth: {tombstone.Data.WealthDescription}";
        natureLabel.Text = $"Nature: {tombstone.Data.Inscription}";
        stateLabel.Text = $"State: {tombstone.State}";
        
        digButton.SetDisabled(tombstone.State == TombstoneState.DugOut);
        
        // TODO: Decide whether a dug-out grave can also be repaired.
        repairButton.SetDisabled(tombstone.State != TombstoneState.Broken);
        
        infoLayout.UpdateLayout();
    }
    
    private void HandleDigClick()
    {
        if (tombstone == null)
            return;
        
        OnDigButton?.Invoke(tombstone);
    }
    
    private void HandleRepairClick()
    {
        if (tombstone == null)
            return;
        
        OnRepairButton?.Invoke(tombstone);
    }

    private void HandleCloseButton()
    {
        OnCloseButton?.Invoke();
    }
    
}