using System;
using System.Collections;
using System.Collections.Generic;
using GraveDigger.Core;
using GraveDigger.GUI.Elements;
using GraveDigger.GUI.Layouts;
using GraveDigger.Items;
using GraveDigger.Systems;
using GUI.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraveDigger.GUI.Windows;

public class TradeWindow : Window
{
    private readonly InventoryView playerInventoryView;
    private readonly InventoryView merchantInventoryView;
    private readonly HorizontalLayout inventoriesLayout;
    private readonly HorizontalLayout buttonsLayout;
    
    private readonly Label playerHintLabel;
    private readonly Label merchantHintLabel;
    private readonly HorizontalLayout hintsLayout;
    
    private readonly Button closeButton;
    
    private readonly Label tradeLabel;
    private const float TradeResultDuration = 3000f;
    private float tradeResultTimer = 0f;
    
    private ItemData selectedItem;
    private Action<ItemData, int> selectedAction;
    
    private Inventory playerInventory;
    private Inventory merchantInventory;

    private int hintPadding = 500;
    
    private InventorySlot draggedSlot;
    private bool isDragging;
    
    private InventoryEntry itemBeingDragged;
    
    public event Action<ItemData, int> DiscardRequested;
    public event Action<ItemData, int> UseRequested;
    public event Action<ItemData, int> SellRequested;
    public event Action<ItemData, int> BuyRequested;
    public event Action OnCloseButton;

    public TradeWindow(Rectangle parentBounds) : base(parentBounds)
    {
        int width = 1400;
        int height = 900;
        int x = parentBounds.X + (parentBounds.Width - width) / 2;
        int y = parentBounds.Y + (parentBounds.Height - height) / 2;
        Bounds = new Rectangle(x, y, width, height);

        int borderWidth = 45;
        
        int halfWidth = (int) (Bounds.Width * 0.5f) - borderWidth;
        int buttonsHeight = 120;
        int hintHeight = 50;
        
        playerInventoryView = CreateElement<InventoryView>();
        playerInventoryView.SetSize(halfWidth, Bounds.Height - buttonsHeight - hintHeight);
        
        merchantInventoryView = CreateElement<InventoryView>();
        merchantInventoryView.SetSize(halfWidth, Bounds.Height - buttonsHeight - hintHeight);

        Rectangle inventoryRect = new Rectangle(Bounds.X, Bounds.Y + 40, 
            Bounds.Width, Bounds.Height - buttonsHeight - hintHeight);
        inventoriesLayout = new HorizontalLayout(inventoryRect);
        
        inventoriesLayout.AddElement(playerInventoryView);
        inventoriesLayout.AddElement(merchantInventoryView);
        
        playerHintLabel = CreateElement<Label>();
        playerHintLabel.Text = "Drag to Sell";

        merchantHintLabel = CreateElement<Label>();
        merchantHintLabel.Text = "Drag to Buy";
        
        tradeLabel = CreateElement<Label>();
        tradeLabel.Text = "";
        tradeLabel.Visible = false;
        
        Rectangle hintsRect = new Rectangle(Bounds.X, Bounds.Bottom - 200, Bounds.Width, 50);
        hintsLayout = new HorizontalLayout(hintsRect);
        
        hintsLayout.HorizontalPadding = hintPadding; 

        hintsLayout.AddElement(playerHintLabel);
        hintsLayout.AddElement(tradeLabel);
        hintsLayout.AddElement(merchantHintLabel);
        
        closeButton = CreateButton("Close", 200, 60, HandleCloseClick);
        
        buttonsLayout = new HorizontalLayout(Bounds);
        buttonsLayout.SetPosition(Bounds.X, Bounds.Bottom - buttonsHeight - 20);
        
        buttonsLayout.AddElement(closeButton);

        playerInventoryView.ContextMenuRequested += OpenPlayerContextMenu;
        merchantInventoryView.ContextMenuRequested += OpenMerchantContextMenu;
        
        RefreshLayout();
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime); 

        MouseState mouse = Mouse.GetState();

        if (!isDragging && mouse.LeftButton == ButtonState.Pressed)
        {
            var slot = FindSlotUnderMouse(mouse.Position);
            if (slot != null && !slot.IsEmpty())
            {
                draggedSlot = slot;
                itemBeingDragged = slot.GetEntry(); 
                isDragging = true;
            }
        }

        if (isDragging && mouse.LeftButton == ButtonState.Released)
        {
            if (draggedSlot != null)
            {
                var targetSlot = FindSlotUnderMouse(mouse.Position);
            
                if (targetSlot != null && targetSlot.IsPlayerSlot != draggedSlot.IsPlayerSlot)
                {
                    var entry = draggedSlot.GetEntry();
                    if (draggedSlot.IsPlayerSlot)
                        Sell(entry); 
                    else
                        Buy(entry);  
                }
            }
        
            isDragging = false;
            draggedSlot = null;
            itemBeingDragged = null;
        }

        TradeResultTimer(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        if (isDragging && itemBeingDragged != null)
        {
            MouseState mouse = Mouse.GetState();
            Texture2D texture = SpriteManager.GetSprite(itemBeingDragged.ItemData.SpriteName).Texture;
            
            Rectangle destination = new Rectangle(mouse.X - 50, mouse.Y - 50, 100, 100);
            
        
            spriteBatch.Draw(texture, destination, Color.White);
        }
    }
    
    protected override void RefreshLayout()
    {
        inventoriesLayout.UpdateLayout();
        buttonsLayout.UpdateLayout();
        hintsLayout.UpdateLayout();
    }
    
    public void SetInventories(Inventory playerInventory, Inventory merchantInventory)
    {
        UnsubscribeInventories();

        this.playerInventory = playerInventory;
        this.merchantInventory = merchantInventory;

        if (this.playerInventory != null)
            this.playerInventory.Changed += RefreshInventories;

        if (this.merchantInventory != null)
            this.merchantInventory.Changed += RefreshInventories;

        RefreshInventories();
    }
    
    protected override void HandleQuantityConfirmed(int amount)
    {
        if (selectedItem == null || selectedAction == null)
            return;
        
        selectedAction?.Invoke(selectedItem, amount);
        
        selectedItem = null;
        selectedAction = null;
    }
    
    private void UnsubscribeInventories()
    {
        if (playerInventory != null)
            playerInventory.Changed -= RefreshInventories;

        if (merchantInventory != null)
            merchantInventory.Changed -= RefreshInventories;
    }
    
    private void RefreshInventories()
    {
        playerInventoryView.SetTitle("Grave Digger");
        playerInventoryView.SetInventory(playerInventory);
        
        merchantInventoryView.SetTitle("Merchant");
        merchantInventoryView.SetInventory(merchantInventory);
    }
    
    private void HandleCloseClick()
    {
        contextMenu.Hide();
        quantitySelector.Hide();
        OnCloseButton?.Invoke();
    }
    
    private void OpenPlayerContextMenu(Point position, InventoryEntry entry)
    {
        List<ContextMenuAction> actions = new();
        
        actions.Add(new("Sell", () => Sell(entry)));
        
        if (entry.ItemData is FoodItemData)
            actions.Add(new ContextMenuAction("Use", () => Use(entry)));

        actions.Add(new("Discard", () => Discard(entry)));
        
        contextMenu.Show(new Vector2(position.X, position.Y), actions);
    }
    
    private void OpenMerchantContextMenu(Point position, InventoryEntry entry)
    {
        List<ContextMenuAction> actions = new();
        
        actions.Add(new("Buy", () => Buy(entry)));
        
        contextMenu.Show(new Vector2(position.X, position.Y), actions);
    }

    private void Sell(InventoryEntry entry)
    {
        RequestQuantity($"Sell {entry.ItemData.Name}", entry, SellRequested);
    }
    
    private void Buy(InventoryEntry entry)
    {
        RequestQuantity($"Buy {entry.ItemData.Name}", entry, BuyRequested);
    }
    
    private void Use(InventoryEntry entry)
    {
        RequestQuantity($"Use {entry.ItemData.Name}", entry, UseRequested);
    }
    
    private void Discard(InventoryEntry entry)
    {
        RequestQuantity($"Discard {entry.ItemData.Name}", entry, DiscardRequested);
    }
    
    private void RequestQuantity(string title, InventoryEntry entry, Action<ItemData, int> action)
    {
        contextMenu.Hide();
        playerInventoryView.ResetInteraction();
        merchantInventoryView.ResetInteraction();
        
        selectedItem = entry.ItemData;
        selectedAction = action;
        
        quantitySelector.Show(title, 1, entry.Quantity);
    }
    
    private InventorySlot FindSlotUnderMouse(Point mousePos)
    {
        foreach (var slot in playerInventoryView.GetAllSlots()) 
            if (slot.Bounds.Contains(mousePos)) { slot.IsPlayerSlot = true; return slot; }
        
        foreach (var slot in merchantInventoryView.GetAllSlots()) 
            if (slot.Bounds.Contains(mousePos)) { slot.IsPlayerSlot = false; return slot; }
        
        return null;
    }

    public void ShowTradeResult(TradeResult result)
    {
        string message = result switch
        {
            TradeResult.Success => "Done.",
            TradeResult.ItemNotFound => "Item not found.",
            TradeResult.InvalidQuantity => "Invalid amount.",
            TradeResult.NotEnoughMoney => "Not enough money.",
            TradeResult.NotEnoughInventorySpace => "No space.",
            _ => "Failed."
        };
        tradeLabel.Text = message;
        tradeLabel.Color =
            result == TradeResult.Success
                ? Color.DarkSeaGreen
                : Color.Red;
        tradeLabel.Visible = true;
        hintsLayout.HorizontalPadding = (int)((hintPadding - tradeLabel.Bounds.Width) * 0.5f);
        hintsLayout.UpdateLayout();

        tradeResultTimer = TradeResultDuration;
    }

    private void HideTradeResult()
    {
        tradeLabel.Visible = false;
        tradeLabel.Text = "";
        hintsLayout.HorizontalPadding = hintPadding;
        hintsLayout.UpdateLayout();
        tradeResultTimer = 0;
    }
    
    private void TradeResultTimer(GameTime gameTime)
    {
        if (tradeLabel.Visible)
        {
            tradeResultTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (tradeResultTimer <= 0)
            {
                HideTradeResult();
            }
        }
    }
}