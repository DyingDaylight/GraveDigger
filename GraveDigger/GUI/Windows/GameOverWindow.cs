using System;
using System.Collections.Generic;
using GraveDigger.Core;
using GraveDigger.GUI.Elements;
using GraveDigger.GUI.Layouts;
using GraveDigger.Systems;
using GUI.Windows;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Windows;

public class GameOverWindow :  Window
{
    private const float FrameDuration = 3f;
    
    private readonly Image image;
    
    private readonly Label resultTextLine1;
    private readonly Label resultTextLine2;
    private readonly Label resultTextLine3;
    
    private readonly Button restartButton;
    private readonly Button closeButton;
    
    private readonly HorizontalLayout buttonsLayout;
    private readonly HorizontalLayout contentLayout;
    private readonly VerticalLayout textLayout;
    private readonly VerticalLayout imageLayout;
    
    private float frameTimer;
    private int currentFrame;
    private bool sequenceFinished;
    
    private readonly Label[] resultLabels;
    
    private GameResult result;
    
    public event Action RestartButtonPressed;
    public event Action ExitButtonPressed;
    
    private readonly Dictionary<GameResult, string[]> finalMessages = new()
    {
        [GameResult.LoseHunger] = new[]
        {
            "You gave everything to keep \nthe cemetery alive.",
            "But eventually, there was nothing \nleft for yourself.",
            "You starved to death."
        },

        [GameResult.LoseReputation] = new[]
        {
            "People stopped coming.",
            "The graves were forgotten.",
            "Your cemetery was abandoned."
        },

        [GameResult.Win] = new[]
        {
            "You became the most respected \ngravedigger.",
            "You died wealthy and honored...",
            "But even your grave couldn't \nescape another gravedigger."
        }
    };
    
    private readonly Dictionary<GameResult, string[]> finalImages = new()
    {
        [GameResult.LoseHunger] = new[]
        {
            "hungerResult1",
            "hungerResult2",
            "hungerResult3"
        },

        [GameResult.LoseReputation] = new[]
        {
            "reputationResult1",
            "reputationResult2",
            "reputationResult3"
        },

        [GameResult.Win] = new[]
        {
            "winResult1",
            "winResult2",
            "winResult3"
        }
    };
    
    
    public GameOverWindow(Rectangle parentBounds) : base(parentBounds)
    {
        resultTextLine1 = CreateElement<Label>();
        resultTextLine2 = CreateElement<Label>();
        resultTextLine3 = CreateElement<Label>();
        
        resultLabels =
        [
            resultTextLine1,
            resultTextLine2,
            resultTextLine3
        ];
        
        image = CreateElement<Image>();
        image.SetSize(270, 449); 
        
        int buttonWidth = 270;
        int buttonHeight = 80;
        
        restartButton = CreateButton("Restart", buttonWidth, buttonHeight, HandleRestartButton);
        closeButton = CreateButton("Exit", buttonWidth, buttonHeight, HandleExitButton);
        
        buttonsLayout = new HorizontalLayout(Bounds);
        buttonsLayout.HorizontalPadding = 20;
        buttonsLayout.SetPosition(Bounds.X, Bounds.Bottom - 200);

        buttonsLayout.AddElement(restartButton);
        buttonsLayout.AddElement(closeButton);
        
        Rectangle textBounds = new Rectangle(Bounds.X + 70, Bounds.Y, (int) ((Bounds.Width - 70) * 0.6f), Bounds.Height - 200);
        textLayout = new VerticalLayout(textBounds);
        textLayout.VerticalPadding = 40;
        textLayout.Alignment = VerticalLayout.HorizontalAlignment.Left;
       
        textLayout.AddElement(resultTextLine1);
        textLayout.AddElement(resultTextLine2);
        textLayout.AddElement(resultTextLine3);
        
        Rectangle imageBounds = new Rectangle(textBounds.X + textBounds.Width, Bounds.Y, (int) ((Bounds.Width) * 0.3f), Bounds.Height - 200);
        imageLayout = new VerticalLayout(imageBounds);
        imageLayout.AddElement(image);

        Rectangle contentBounds = new Rectangle(Bounds.X + 70, Bounds.Y + 55, Bounds.Width - 140, Bounds.Height - 200);
        contentLayout = new HorizontalLayout(contentBounds);
        contentLayout.HorizontalPadding = 20;
        contentLayout.AddElement(textLayout);
        contentLayout.AddElement(imageLayout);
        
        RefreshLayout();
    }
    
    public void SetResult(GameResult result)
    {
        this.result = result;

        currentFrame = 0;
        frameTimer = 0f;
        sequenceFinished = false;

        foreach (Label label in resultLabels)
            label.Text = string.Empty;

        restartButton.Visible = false;
        closeButton.Visible = false;

        ShowCurrentFrame();
    }
    
    protected override void RefreshLayout()
    {
        buttonsLayout.UpdateLayout();

        contentLayout.UpdateLayout();
        textLayout.UpdateLayout();
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (sequenceFinished)
            return;

        frameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (frameTimer < FrameDuration)
            return;

        frameTimer = 0f;
        currentFrame++;

        if (currentFrame >= finalMessages[result].Length)
        {
            FinishSequence();
            return;
        }

        ShowCurrentFrame();
    }
    
    private void HandleRestartButton()
    {
        RestartButtonPressed?.Invoke();
    }

    private void HandleExitButton()
    {
        ExitButtonPressed?.Invoke();
    }
    
    private void ShowCurrentFrame()
    {
        string[] messages = finalMessages[result];
        string[] images = finalImages[result];

        resultLabels[currentFrame].Text = messages[currentFrame];

        image.SetImage(SpriteManager.GetSprite(images[currentFrame]).Texture);

        RefreshLayout();
    }
    
    private void FinishSequence()
    {
        sequenceFinished = true;

        restartButton.Visible = true;
        closeButton.Visible = true;

        RefreshLayout();
    }
}