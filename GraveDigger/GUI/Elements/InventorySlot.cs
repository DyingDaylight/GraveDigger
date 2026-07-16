using System;
using System.Collections.Generic;
using GraveDigger.Core;
using GraveDigger.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.GUI.Elements;

public class InventorySlot : ClickableUIElement
{
    private enum State
    {
        Normal,
        Hover,
        Pressed,
        Disabled
    }
    
    private State currentState;
    
    private readonly Image background;
    private readonly Image hoverEffect;
    private readonly Image icon;

    private readonly Image priceIcon;
    private readonly Label price;
    private readonly Label amount;
    private readonly Tooltip tooltip;
    
    private readonly List<UIElement> children = new();
    
    private readonly Vector2 padding = new(20, 20);
    
    private InventoryEntry inventoryEntry;
    
    public event Action<Point, InventoryEntry> OnItemRightClicked;
    
    
    public InventorySlot()
    {
        RightClicked += HandleRightClick;
        
        SetSize(100, 100);
        
        background = Create<Image>();
        background.SetSize(Bounds.Width, Bounds.Height);
        background.SetImage(SpriteManager.GetSprite("pixel").Texture);
        background.SetTint(Color.DarkOliveGreen);
        
        hoverEffect = Create<Image>();
        hoverEffect.SetSize(Bounds.Width, Bounds.Height);
        hoverEffect.SetImage(SpriteManager.GetSprite("pixel").Texture);
        hoverEffect.SetTint(Color.GreenYellow);
        
        icon = Create<Image>();
        icon.SetSize(Bounds.Width - (int) padding.X,
                    Bounds.Height - (int) padding.Y);
        
        priceIcon = Create<Image>();
        priceIcon.SetSize(50,50);
        priceIcon.SetImage(SpriteManager.GetSprite("Coin").Texture);
        
        price = Create<Label>();
        price.Color = Color.Black;
        
        amount = Create<Label>();
        amount.Color = Color.Black;

        tooltip = Create<Tooltip>();
    }

    public void SetData(InventoryEntry? inventoryEntry)
    {
        this.inventoryEntry = inventoryEntry;
        RefreshContent();
        UpdateElementPositions();
    }

    public override void Start()
    {
        base.Start();
        foreach (UIElement child in children)
            child.Start();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (IsEmpty())
        {
            currentState = State.Disabled;
        }
        else
        {
            UpdateInteraction();

            currentState =
                IsLeftPressed || IsRightPressed
                    ? State.Pressed
                    : IsHovered
                        ? State.Hover
                        : State.Normal;
        }
        
        UpdateElements(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        
        background.Draw(spriteBatch);

        if (currentState == State.Hover)
        {
            hoverEffect.Draw(spriteBatch);
            tooltip.Draw(spriteBatch);
        }
        
        if (IsEmpty())
            return;

        icon.Draw(spriteBatch);
        priceIcon.Draw(spriteBatch);
        price.Draw(spriteBatch);
        amount.Draw(spriteBatch);
    }

    public override void SetPosition(int x, int y)
    {
        base.SetPosition(x, y);
        UpdateElementPositions();
    }
    
    public override void ResetInteraction()
    {
        base.ResetInteraction();

        currentState = IsEmpty()
            ? State.Disabled
            : State.Normal;
    }

    private T Create<T>() where T : UIElement, new()
    {
        T element = new T();
        children.Add(element);
        return element;
    }

    private void RefreshContent()
    {
        if (IsEmpty())
        {
            icon.SetImage(null);
            price.Text = string.Empty;
            amount.Text = string.Empty;
            tooltip.SetTooltip(string.Empty);
            return;
        }
        
        string spriteName = inventoryEntry.ItemData.SpriteName;
        icon.SetImage(SpriteManager.GetSprite(spriteName).Texture);

        price.Text = inventoryEntry.ItemData.Price.ToString();
        amount.Text = inventoryEntry.Quantity.ToString();
        
        string tooltipText = inventoryEntry.ItemData.ToString();
        tooltip.SetTooltip(tooltipText);
    }

    private void UpdateElementPositions()
    {
        background.SetPosition(Bounds.X, Bounds.Y);
        hoverEffect.SetPosition(Bounds.X, Bounds.Y);
        
        icon.SetPosition(
            Bounds.X + (int) ((Bounds.Width - icon.Bounds.Width) * 0.5f),
            Bounds.Y + (int) ((Bounds.Height - icon.Bounds.Height) * 0.5f));
        
        priceIcon.SetPosition(Bounds.X, Bounds.Y);
        
        price.SetPosition(Bounds.X + priceIcon.Bounds.Width, Bounds.Y);
        amount.SetPosition(
            Bounds.Right - amount.Bounds.Width, 
            Bounds.Bottom - amount.Bounds.Height);
        tooltip.SetPosition(
            (int) (Bounds.X + (Bounds.Width - tooltip.Bounds.Width) * 0.5f), 
            Bounds.Y - tooltip.Bounds.Height);
    }
    
    private void UpdateElements(GameTime gameTime)
    {
        background.Update(gameTime);
        hoverEffect.Update(gameTime);

        if (IsEmpty())
            return;
        
        icon.Update(gameTime);
        priceIcon.Update(gameTime);
        price.Update(gameTime);
        amount.Update(gameTime);
        tooltip.Update(gameTime);
    }
    
    private bool IsEmpty()
    {
        return inventoryEntry == null || inventoryEntry.ItemData == null;
    }
    
    private void HandleRightClick(Point position)
    {
        if (!IsEmpty())
            OnItemRightClicked?.Invoke(position, inventoryEntry);
    }
}