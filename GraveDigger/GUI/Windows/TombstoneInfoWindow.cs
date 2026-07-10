using System;
using System.Collections.Generic;
using GraveDigger.Data;
using GraveDigger.GUI.Elements;
using GraveDigger.GUI.Layouts;
using GUI;
using GUI.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.GUI.Windows;

public class TombstoneInfoWindow : Window
{

    private Label nameLabel;
    private Label yearLabel;
    private Label wealthLabel;
    private Label natureLabel;
    private Label stateLabel;

    private HorizontalLayout horizontalLayout;
    private VerticalLayout verticalLayout;
    
    private Button digButton;
    private Button repairButton;
    private Button closeButton;
    
    public event Action OnDigButton;
    public event Action OnRepairButton;
    public event Action OnCloseButton;
    
    public TombstoneInfoWindow()
    {
        Color = Color.DimGray;
        
        nameLabel = new Label();
        elements.Add(nameLabel);
        
        yearLabel = new Label();
        elements.Add(yearLabel);
        
        wealthLabel = new Label();
        elements.Add(wealthLabel);
        
        natureLabel = new Label();
        elements.Add(natureLabel);
        
        stateLabel = new Label();
        elements.Add(stateLabel);
        
        digButton = new Button();
        digButton.SetText("Dig");
        digButton.OnClick += () => OnDigButton?.Invoke();
        elements.Add(digButton);
        
        repairButton = new Button();
        repairButton.SetText("Repair");
        repairButton.OnClick += () => OnRepairButton?.Invoke();
        elements.Add(repairButton);
        
        closeButton = new Button();
        closeButton.SetText("Close");
        closeButton.OnClick += () => OnCloseButton?.Invoke();
        elements.Add(closeButton);
        
        horizontalLayout = new HorizontalLayout(Bounds);
        horizontalLayout.Padding = 20;
        horizontalLayout.PositionY = Bounds.Y + Bounds.Height - 120;
        horizontalLayout.AddElement(digButton);
        horizontalLayout.AddElement(repairButton);
        horizontalLayout.AddElement(closeButton);
        horizontalLayout.CountPositions();
        
        Rectangle bounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height - 120);
        verticalLayout = new VerticalLayout(bounds);
        verticalLayout.Padding = 40;
        verticalLayout.AddElement(nameLabel);
        verticalLayout.AddElement(yearLabel);
        verticalLayout.AddElement(wealthLabel);
        verticalLayout.AddElement(natureLabel);
        verticalLayout.AddElement(stateLabel);
        verticalLayout.CountPositions();
    }

    public override void Update(GameTime gameTime)
    {
        verticalLayout.CountPositions();
        base.Update(gameTime);
    }

    public void SetData(TombstoneData tombstoneData)
    {
        nameLabel.Text = tombstoneData.Name;
        yearLabel.Text = tombstoneData.Years;
        wealthLabel.Text = $"Wealth: {tombstoneData.Wealth}";
        natureLabel.Text = $"Nature: {tombstoneData.Nature}";
        stateLabel.Text = $"State: {tombstoneData.State}";
    }
}