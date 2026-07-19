using GraveDigger.Core;
using GraveDigger.Interactions;
using GraveDigger.Items;
using GraveDigger.Systems;
using GraveDigger.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.Characters;

public class Merchant : Animation, IInteractionOwner
{
    private const float ArrivalThreshold = 10f;
    private const float MovementSpeed = 200f;
    
    private Vector2 offMapPosition = Vector2.Zero;
    private Vector2 onMapPosition = Vector2.Zero;
    
    private MerchantState state = MerchantState.Idle;
    private Vector2 targetPosition = Vector2.Zero;
    
    private bool leaveRequested = false;
    
    public Rectangle InteractionArea => DestRectangle;
    public Inventory Inventory { get; set; }
    public TraderInteraction TraderInteraction { get; set; }

    public Merchant() : base("merchant")
    {
        Inventory = new Inventory();
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
        
        Transform.Position = offMapPosition;
        ChangeState(MerchantState.Arriving);
        
        Transform.Scale = new Vector2(0.7f, 0.7f);
        
        CurrentRow = 1; 
        Stop();
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
            leaveRequested = true;
            return;
        }

        if (state == MerchantState.Trading && leaveRequested)
        {
            leaveRequested = false;
            newState = MerchantState.Leaving;
        }

        switch (newState)
        {
            case MerchantState.Hidden:
                TraderInteraction.IsActive = false;
                break;

            case MerchantState.Arriving:
                targetPosition = onMapPosition;
                TraderInteraction.IsActive = false;
                break;

            case MerchantState.Idle:
                TraderInteraction.IsActive = true;
                break;

            case MerchantState.Trading:
                TraderInteraction.IsActive = false;
                break;

            case MerchantState.Leaving:
                targetPosition = offMapPosition;
                TraderInteraction.IsActive = false;
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
        
        if (direction.Length() <= ArrivalThreshold)
        {
            Transform.Position = targetPosition;
            ReachDestination();
            CurrentRow = 3;
            return;
        }

        direction.Normalize();

        Transform.Position += direction * MovementSpeed * dt;

        if (direction.X != 0)
        {
            CurrentRow = 2;
            SpriteEffect = direction.X > 0
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;
        }
        else
        {
            CurrentRow = direction.Y > 0 ? 1 : 0;
        }

        Play(3);
    }

    private void ReachDestination()
    {
        if (state == MerchantState.Leaving) 
            ChangeState(MerchantState.Hidden);
        else if (state == MerchantState.Arriving)
            ChangeState(MerchantState.Idle);
    }

    public void RefreshInventory(RandomService randomService)
    {
        Inventory.ClearItemsByType<LootItemData>();

        // TODO: add other items
        int amount = randomService.Next(1, 6);
        for (int i = 0; i < amount; i++)
        {
            Inventory.Add(InventoryGenerator.GetRandomFood(randomService));
        }
    }
}