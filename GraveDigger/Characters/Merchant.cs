using System;
using GraveDigger.Core;
using GraveDigger.Interactions;
using GraveDigger.Items;
using GraveDigger.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GraveDigger.Characters;

public class Merchant : Animation, IInteractionOwner
{
    private readonly Vector2 offMapPosition = Vector2.Zero;
    private readonly Vector2 onMapPosition = Vector2.Zero;
    
    private MerchantState state = MerchantState.Idle;
    private Vector2 targetPosition = Vector2.Zero;
    private GameContext gameContext;
    
    private float arrivalThreshold = 0.05f;
    private float movementSpeed = 200f;
    
    public Rectangle InteractionArea { get; }
    public Inventory Inventory { get; private set; }
    public TraderInteraction TraderInteraction { get; private set; }

    public Merchant(GameContext gameContext) : base("merchant")
    {
        this.gameContext = gameContext;
        Inventory = new Inventory();
        
        TraderInteraction = new TraderInteraction(this);
        
        offMapPosition = new Vector2(
            gameContext.WorldSize.X + Width, 
            gameContext.WorldSize.Y + Height);
        onMapPosition = new Vector2(1920, 1080);
    }

    public override void Start()
    {
        base.Start();
        CastShadow = true;
        
        Transform.Position = offMapPosition;
        state = MerchantState.Idle;
        
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
        
        Console.WriteLine($"Merchant Changes from {state} to {newState}");
        
        switch (newState)
        {
            case MerchantState.Idle:
                TraderInteraction.IsActive = true;
                break;
            case MerchantState.Arriving:
                targetPosition = onMapPosition;
                TraderInteraction.IsActive = false;
                break;
            case MerchantState.Leaving:
                targetPosition = offMapPosition;
                TraderInteraction.IsActive = false;
                break;
        }
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
        Vector2 direction = targetPosition - Transform.Position;

        if (direction.Length() <= arrivalThreshold)
        {
            Transform.Position = targetPosition;
            ReachDestination();
            CurrentRow = 3;
            return;
        }

        direction.Normalize();

        Transform.Position += direction * movementSpeed * dt;

        if (direction.X > direction.Y)
        {
            if (direction.X > 0)
            {
                CurrentRow = 2;
                // right
            }
            else
            {
                CurrentRow = 2;
                // left
            }
        }
        else
        {
            if (direction.Y > 0)
            {
                CurrentRow = 1;
            }
            else
            {
                CurrentRow = 0;
            }
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
}