using System;
using GraveDigger;
using GraveDigger.GUI.Elements;
using GraveDigger.Items;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GUI;

public class InventorySlot : ClickableUIElement
{
    public enum State
    {
        Normal,
        Hover,
        Pressed,
        Disabled
    }
    
    private State currentState;
    
    private Image background;
    private Image hoverEffect;
    private Image icon;

    private Image priceIcon;
    private Label price;
    private Label amount;
    private Tooltip tooltip;
    
    private InventoryEntry inventoryEntry;
    
    private Vector2 padding = new Vector2(20, 20);
    
    private ButtonState previousMouseButtonState;
    private bool wasPressedInside;
    
    public event Action<InventoryEntry> OnItemSelected;
    
    public InventorySlot()
    {
        RightClicked += HandleRightClick;
        
        SetSize(100, 100);
        
        background = new Image();
        background.SetSize(Bounds.Width, Bounds.Height);
        background.SetImage(SpriteManager.GetSprite("pixel").Texture);
        background.SetTint(Color.DarkOliveGreen);
        
        hoverEffect = new Image();
        hoverEffect.SetSize(Bounds.Width, Bounds.Height);
        hoverEffect.SetImage(SpriteManager.GetSprite("pixel").Texture);
        hoverEffect.SetTint(Color.GreenYellow);
        
        icon = new Image();
        icon.SetSize(Bounds.Width - (int)padding.X,
            Bounds.Height - (int)padding.Y);
        
        priceIcon = new Image();
        priceIcon.SetSize(50,50);
        price = new Label();
        price.Color = Color.Black;
        amount = new Label();
        amount.Color = Color.Black;

        tooltip = new Tooltip();
    }

    public void SetData(InventoryEntry inventoryEntry)
    {
        this.inventoryEntry = inventoryEntry;
    }

    public override void Start()
    {
        base.Start();
        tooltip.Start();
        
    }

    public override void Update(GameTime gameTime)
    {
        if (!IsEmpty())
        {
            UpdateInteraction();

            currentState =
                IsLeftPressed || IsRightPressed
                    ? State.Pressed
                    : IsHovered
                        ? State.Hover
                        : State.Normal;
        }
        else
        {
            currentState = State.Disabled;
        }

        UpdateElements(gameTime);
    }

    private void UpdateElements(GameTime gameTime)
    {
        background.Update(gameTime);
        hoverEffect.Update(gameTime);

        if (inventoryEntry != null)
        {
            icon.SetImage(SpriteManager.GetSprite(inventoryEntry.ItemData.SpriteName).Texture);
            icon.Update(gameTime);
            
            priceIcon.SetImage(SpriteManager.GetSprite("Coin").Texture);
            priceIcon.SetPosition(Bounds.X,  Bounds.Y);
            
            price.Text = inventoryEntry.ItemData.Price.ToString();
            price.SetPosition(Bounds.X + priceIcon.Bounds.Width, Bounds.Y);
            
            amount.Text = inventoryEntry.Quantity.ToString();
            amount.SetPosition(Bounds.X + Bounds.Width - amount.Bounds.Width,
                Bounds.Y + Bounds.Height - amount.Bounds.Height);
            
            tooltip.SetTooltip(inventoryEntry.ItemData.Name + "\n" + 
                               inventoryEntry.ItemData.Description);
            tooltip.SetPosition((int)(Bounds.X + (Bounds.Width - tooltip.Bounds.Width) * 0.5f), 
                Bounds.Y - tooltip.Bounds.Height);
            
            icon.Update(gameTime);
            priceIcon.Update(gameTime);
            price.Update(gameTime);
            amount.Update(gameTime);
            tooltip.Update(gameTime);
        }
        else
            icon.SetImage(null);
    }

    private bool IsEmpty()
    {
        return inventoryEntry == null || inventoryEntry.ItemData == null;
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
        
        if (inventoryEntry != null)
        {
            icon.Draw(spriteBatch);
            priceIcon.Draw(spriteBatch);
            price.Draw(spriteBatch);
            amount.Draw(spriteBatch);
        }
    }

    public override void SetPosition(int x, int y)
    {
        base.SetPosition(x, y);
        background.SetPosition(x, y);
        hoverEffect.SetPosition(x, y);
        icon.SetPosition(x + (int) ((Bounds.Width - icon.Bounds.Width) * 0.5f),
            y + (int) ((Bounds.Height - icon.Bounds.Height) * 0.5f));
    }
    
    private void HandleRightClick()
    {
        if (!IsEmpty())
            OnItemSelected?.Invoke(inventoryEntry);
    }
}