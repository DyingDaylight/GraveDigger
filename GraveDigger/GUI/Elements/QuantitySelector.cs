using System;
using GraveDigger.Core;
using GraveDigger.GUI.Layouts;
using GUI;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Elements;

public class QuantitySelector : UIContainer
{
    private readonly Label titleLabel;
    private readonly Label quantityLabel;
    
    private readonly Button minusButton;
    private readonly Button plusButton;
    private readonly Button confirmButton;
    private readonly Button allButton;
    private readonly Button cancelButton;
    
    private readonly VerticalLayout mainLayout;
    private readonly HorizontalLayout quantityLayout;
    private readonly HorizontalLayout buttonsLayout;

    private int quantity = 1;
    private int minQuantity = 1;
    private int maxQuantity = 1;
    
    
    public event Action<int> ConfirmRequested;
    public event Action Closed;
    
    public QuantitySelector(Rectangle parentBounds)
    {
        Visible = false;
        
        int width = 600;
        int height = 300;
        int x = (int) (parentBounds.X + (parentBounds.Width - width) * 0.5f);
        int y = (int) (parentBounds.Y + (parentBounds.Height - height) * 0.5f);

        int buttonSize = 60;
        int buttonHeight = 70;
        int buttonWidth = 150;
        
        Bounds = new Rectangle(x, y, width, height);
        
        Color = Color.White;
        Texture = SpriteManager.GetSprite("background").Texture;
        
        titleLabel = CreateElement<Label>();
        titleLabel.Scale = 1.2f;
        
        quantityLabel = CreateElement<Label>();
        quantityLabel.Scale = 1.5f;
        UpdateQuantity();
        
        plusButton = CreateButton("+", buttonSize, buttonSize, IncreaseQuantity);

        minusButton = CreateButton("-", buttonSize, buttonSize, DecreaseQuantity);

        confirmButton = CreateButton("Confirm", buttonWidth, buttonHeight, Confirm);
        
        allButton = CreateButton("All", buttonWidth, buttonHeight, ChooseAll);
        
        cancelButton = CreateButton("Cancel", buttonWidth, buttonHeight, Hide);
        
        Rectangle quantityBounds = new Rectangle(x, y, width, buttonSize);
        quantityLayout = new HorizontalLayout(quantityBounds);
        quantityLayout.HorizontalPadding = 70;
        quantityLayout.AddElement(minusButton);
        quantityLayout.AddElement(quantityLabel);
        quantityLayout.AddElement(plusButton);
        
        Rectangle buttonsBounds = new Rectangle(x, y, width, buttonHeight);
        buttonsLayout = new HorizontalLayout(buttonsBounds);
        buttonsLayout.HorizontalPadding = 30;
        buttonsLayout.AddElement(confirmButton);
        buttonsLayout.AddElement(allButton);
        buttonsLayout.AddElement(cancelButton);
        
        mainLayout = new VerticalLayout(Bounds);
        mainLayout.VerticalPadding = 25;
        mainLayout.Margins = new Vector2(0, 20);
        mainLayout.AddElement(titleLabel);
        mainLayout.AddElement(quantityLayout);
        mainLayout.AddElement(buttonsLayout);
        
        Refresh();
    }

    public void Show(string title, int min, int max)
    {
        if (min < 1)
            min = 1;

        if (max < min)
            max = min;
        
        titleLabel.Text = title;
        minQuantity = min;
        maxQuantity = max;
        quantity = minQuantity;
        UpdateQuantity();
        
        Refresh();
        
        Visible = true;
    }

    public void Hide()
    {
        Visible = false;
        Closed?.Invoke();
    }

    private void Refresh()
    {
        quantityLayout.UpdateLayout();
        buttonsLayout.UpdateLayout();
        mainLayout.UpdateLayout();
        
        quantityLabel.CenterIn(new Rectangle(
            minusButton.Bounds.X + minusButton.Bounds.Width, 
            minusButton.Bounds.Y,
            plusButton.Bounds.X - (minusButton.Bounds.X + minusButton.Bounds.Width),
            minusButton.Bounds.Height));
    }

    private void IncreaseQuantity()
    {
        quantity++;
        quantity = Math.Clamp(quantity, minQuantity, maxQuantity);
        UpdateQuantity();
    }

    private void DecreaseQuantity()
    {
        quantity--;
        quantity = Math.Clamp(quantity, minQuantity, maxQuantity);
        UpdateQuantity();
    }
    
    private void ChooseAll()
    {
        quantity = maxQuantity;
        UpdateQuantity();
    }

    private void Confirm()
    {
        int confirmedQuantity = quantity;
        Hide();
        ConfirmRequested?.Invoke(confirmedQuantity);
    }

    private void UpdateQuantity()
    {
        quantityLabel.Text = $"{quantity}/{maxQuantity}";
    }
}