using System;
using GraveDigger.Core;
using GraveDigger.Interactions;
using GraveDigger.Items;
using GraveDigger.Props;
using GraveDigger.Systems;
using GraveDigger.Utils;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.Characters;

public class Merchant : Animation, IInteractionOwner, IHasCollider
{
    private const float ArrivalThreshold = 10f;
    private const float MovementSpeed = 200f;
    
    private const int MoveUpRow = 0;
    private const int MoveDownRow = 1;
    private const int MoveSidewaysRow = 2;
    private const int IdleRow = 3;
    
    private Vector2 offMapPosition = Vector2.Zero;
    private Vector2 onMapPosition = Vector2.Zero;
    
    private MerchantState state = MerchantState.Hidden;
    private Vector2 targetPosition = Vector2.Zero;
    
    private bool pendingLeave = false;
    
    public Rectangle InteractionArea => DestRectangle;
    public Inventory Inventory { get; set; }
    public TraderInteraction TraderInteraction { get; set; }
    
    public Collider Collider { get; }

    public Merchant() : base("merchant")
    {
        Inventory = new Inventory();
        Collider = new Collider(this);
        Collider.Layer = CollisionLayer.Character;
        Collider.Mask = CollisionLayer.Player;
    }

    public void SetOffMapPosition(Vector2 position)
    {
        offMapPosition = position;
    }
    
    public void SetOnMapPosition(Vector2 position)
    {
        onMapPosition = position;
    }

    public override void Start()
    {
        base.Start();
        CastShadow = true;
        Transform.Scale = new Vector2(0.45f, 0.45f);
        
        Transform.Position = offMapPosition;
        ChangeState(MerchantState.Arriving);
        
        Play(3);
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
        UpdateMovement(dt);
        UpdateSortingOrder();
    }

    public void ChangeState(MerchantState newState)
    {
        if (state == newState)
            return;

        if (state == MerchantState.Trading && newState == MerchantState.Leaving)
        {
            pendingLeave = true;
            return;
        }

        if (state == MerchantState.Trading && pendingLeave)
        {
            pendingLeave = false;
            newState = MerchantState.Leaving;
        }

        switch (newState)
        {
            case MerchantState.Hidden:
                TraderInteraction.IsActive = false;
                Stop();
                break;

            case MerchantState.Arriving:
                Transform.Position = offMapPosition; 
                targetPosition = onMapPosition;
                TraderInteraction.IsActive = false;
                Play(3);
                break;

            case MerchantState.Idle:
                TraderInteraction.IsActive = true;
                SetIdleFrontFacing();
                break;

            case MerchantState.Trading:
                TraderInteraction.IsActive = false;
                SetIdleFrontFacing();
                break;

            case MerchantState.Leaving:
                targetPosition = offMapPosition;
                TraderInteraction.IsActive = false;
                Play(3);
                break;
        }

        state = newState;
    }

    public void SetHighlighted(bool highlighted)
    {
        Highlighted = highlighted;
    }
    
    private void UpdateSortingOrder()
    {
        SortingOrder = SortingUtility.CalculateByY(Bottom);
    }
    
    private void UpdateMovement(float dt)
    {
        if (state is MerchantState.Idle or MerchantState.Hidden or MerchantState.Trading)
            return;
        
        Vector2 direction = targetPosition - Transform.Position;
        float distance = direction.Length();
        float movementDistance = MovementSpeed * dt;
        
        if (distance <= Math.Max(ArrivalThreshold, movementDistance))
        {
            Transform.Position = targetPosition;
            ReachDestination();
            return;
        }

        direction.Normalize();

        Transform.Position += direction * movementDistance;

        UpdateMovementAnimation(direction);
    }

    private void ReachDestination()
    {
        if (state == MerchantState.Leaving) 
            ChangeState(MerchantState.Hidden);
        else if (state == MerchantState.Arriving)
            ChangeState(MerchantState.Idle);
    }

    private void SetIdleFrontFacing()
    {
        CurrentRow = MoveDownRow;          
        SpriteEffect = SpriteEffects.None;  
        currentColumn = 0;                 
        Stop();                           
    }
    
    public void RefreshInventory(RandomService randomService,
        Func<DecorationType, bool> hasLockedDecorations)
    {
        Inventory.ClearItemsByType<LootItemData>();

        InventoryGenerator.AddMerchantItems(
            Inventory, randomService, hasLockedDecorations);
    }
    
    private void UpdateMovementAnimation(Vector2 direction)
    {
        if (Math.Abs(direction.X) > Math.Abs(direction.Y))
        {
            CurrentRow = MoveSidewaysRow;
            SpriteEffect = direction.X > MoveUpRow
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;
        }
        else
        {
            CurrentRow = direction.Y > MoveUpRow ? MoveDownRow : MoveUpRow;
            SpriteEffect = SpriteEffects.None;
        }
    }
}