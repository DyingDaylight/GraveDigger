using System;
using GraveDigger.Core;
using GraveDigger.GraveSites;
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
    private readonly Label hintLabel;

    private readonly Button digButton;
    private readonly Button repairButton;
    private readonly Button closeButton;
    
    private readonly HorizontalLayout buttonsLayout;
    private readonly VerticalLayout infoLayout;
    
    private Tombstone? tombstone;
    private GraveSite? graveSite;
    private bool hasEnoughMoney;
    
    public event Action<Tombstone> DigButtonPressed;
    public event Action<Tombstone> RepairButtonPressed;
    public event Action OnCloseButton;
    
    public TombstoneInfoWindow(Rectangle parentBounds) : base(parentBounds)
    {
        nameLabel = CreateElement<Label>();
        yearLabel = CreateElement<Label>();
        wealthLabel = CreateElement<Label>();
        natureLabel = CreateElement<Label>();
        stateLabel = CreateElement<Label>();
        hintLabel = CreateElement<Label>();

        int buttonWidth = 270;
        int buttonHeight = 80;
        
        digButton = CreateButton("Dig", buttonWidth, buttonHeight, HandleDigClick);
        repairButton = CreateButton("Repair", buttonWidth, buttonHeight, HandleRepairClick);
        repairButton.SetIcon(SpriteManager.GetSprite("Coin").Texture);
        closeButton = CreateButton("Close", buttonWidth, buttonHeight, HandleCloseButton);
        
        buttonsLayout = new HorizontalLayout(Bounds);
        buttonsLayout.HorizontalPadding = 20;
        buttonsLayout.SetPosition(Bounds.X, Bounds.Bottom - 190);
        
        buttonsLayout.AddElement(digButton);
        buttonsLayout.AddElement(repairButton);
        buttonsLayout.AddElement(closeButton);
        
        Rectangle contentBounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height - 120);
        infoLayout = new VerticalLayout(contentBounds);
        infoLayout.VerticalPadding = 40;
       
        infoLayout.AddElement(nameLabel);
        infoLayout.AddElement(yearLabel);
        infoLayout.AddElement(wealthLabel);
        infoLayout.AddElement(natureLabel);
        infoLayout.AddElement(stateLabel);
        infoLayout.AddElement(hintLabel);

        RefreshLayout();
    }

    public void SetData(GraveSite graveSite, bool hasEnoughMoney)
    {
        this.graveSite = graveSite;
        this.hasEnoughMoney = hasEnoughMoney;
        tombstone = graveSite.Tombstone;
        RefreshContent();
    }

    public void Refresh(bool hasEnoughMoney)
    {
        if (tombstone == null || graveSite == null)
            return;
        
        this.hasEnoughMoney = hasEnoughMoney;
        RefreshContent();
    }
    
    protected override void RefreshLayout()
    {
        buttonsLayout.UpdateLayout();
        infoLayout.UpdateLayout();
    }

    private void RefreshContent()
    {
        if (tombstone == null || graveSite == null)
            return;
        
        nameLabel.Text = tombstone.Data.Name;
        yearLabel.Text = tombstone.Data.LifeYears;
        wealthLabel.Text = $"Wealth: {tombstone.Data.WealthDescription}";
        natureLabel.Text = $"Nature: {tombstone.Data.Inscription}";
        stateLabel.Text = $"State: {graveSite.State}";
        
        digButton.SetDisabled(graveSite.State == GraveSiteState.DugOut);
        
        repairButton.SetDisabled(graveSite.State == GraveSiteState.Intact || !hasEnoughMoney);
        if (graveSite.State != GraveSiteState.Intact)
        {
            repairButton.SetText($"Repair ({graveSite.RepairCost})");
            if (!hasEnoughMoney)
            {
                hintLabel.Text = "Not enough money to repair";
                hintLabel.Color = Color.Red;
                hintLabel.Visible = true;
            }
            else
            {
                hintLabel.Visible = false;
            }
        }
        else
        {
            repairButton.SetText("Repair");
            hintLabel.Visible = false;
        }
        
        RefreshLayout();
    }
    
    private void HandleDigClick()
    {
        if (tombstone == null)
            return;
        
        DigButtonPressed?.Invoke(tombstone);
    }
    
    private void HandleRepairClick()
    {
        if (tombstone == null || !hasEnoughMoney)
            return;
        
        RepairButtonPressed?.Invoke(tombstone);
    }

    private void HandleCloseButton()
    {
        OnCloseButton?.Invoke();
    }
}