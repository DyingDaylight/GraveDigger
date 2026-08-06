using System;
using GraveDigger.Core;
using GraveDigger.GUI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraveDigger.GUI.Components;

public class VolumeSlider : UIContainer
{
    private readonly Label titleLabel;
    private readonly Label percentLabel;
    private readonly Image lineIcon;
    private readonly Image sliderIcon;
    private readonly SpriteFont font;

    private readonly int lineWidth = 400;
    private readonly int lineHeight = 30;
    private readonly int sliderWidth = 50;
    private readonly int sliderHeight = 50;

    private bool isDragging;

    public float Value { get; private set; } 

    public event Action<float>? OnValueChanged;

    public VolumeSlider(string title, float initialValue, Texture2D lineTexture, Texture2D sliderTexture, SpriteFont font)
    {
        this.font = font;
        Value = MathHelper.Clamp(initialValue, 0f, 1f);
        
        SetSize(lineWidth, 90);

        titleLabel = CreateElement<Label>();
        titleLabel.Text = title;

        lineIcon = CreateElement<Image>();
        lineIcon.SetSize(lineWidth, lineHeight);
        lineIcon.SetImage(lineTexture ?? SpriteManager.GetSprite("ReputationLineIcon").Texture);

        sliderIcon = CreateElement<Image>();
        sliderIcon.SetSize(sliderWidth, sliderHeight);
        sliderIcon.SetImage(sliderTexture ?? SpriteManager.GetSprite("ReputationSliderIcon").Texture);

        percentLabel = CreateElement<Label>();
        percentLabel.Text = $"{(int)(Value * 100)}%";

        RefreshLayout();
    }

    public override void SetPosition(int x, int y)
    {
        base.SetPosition(x, y);
        RefreshLayout();
    }

    protected override void RefreshLayout()
    {
        base.RefreshLayout();

        int lineX = Bounds.X;
        int lineY = Bounds.Y + 35; 
        int lineCenterX = lineX + (lineWidth / 2);

        lineIcon?.SetPosition(lineX, lineY);

        if (titleLabel != null)
        {
            int titleX = lineCenterX - 50; 
            if (font != null && !string.IsNullOrEmpty(titleLabel.Text))
            {
                Vector2 textSize = font.MeasureString(titleLabel.Text);
                titleX = lineCenterX - (int)(textSize.X * 0.5f);
            }
            titleLabel.SetPosition(titleX, Bounds.Y);
        }

        int sliderY = lineY - (sliderHeight - lineHeight) / 2;
        
        float sliderCenterX = MathHelper.Lerp(
            lineX + sliderWidth * 0.5f,
            lineX + lineWidth - sliderWidth * 0.5f,
            Value);

        int sliderX = (int)(sliderCenterX - sliderWidth * 0.5f);
        sliderIcon?.SetPosition(sliderX, sliderY);

        if (percentLabel != null)
        {
            int percentX = lineCenterX - 15;
            if (font != null && !string.IsNullOrEmpty(percentLabel.Text))
            {
                Vector2 percentSize = font.MeasureString(percentLabel.Text);
                percentX = lineCenterX - (int)(percentSize.X * 0.5f);
            }
            percentLabel.SetPosition(percentX, lineY + lineHeight + 10);
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        MouseState mouse = Mouse.GetState();
        Point mousePos = mouse.Position;

        Rectangle lineBounds = lineIcon.Bounds;
        Rectangle sliderBounds = sliderIcon.Bounds;

        if (mouse.LeftButton == ButtonState.Pressed)
        {
            if (!isDragging && (sliderBounds.Contains(mousePos) || lineBounds.Contains(mousePos)))
            {
                isDragging = true;
            }

            if (isDragging)
            {
                float relativeX = mousePos.X - lineBounds.Left;
                float newValue = MathHelper.Clamp(relativeX / lineBounds.Width, 0f, 1f);

                if (Math.Abs(newValue - Value) > 0.001f)
                {
                    Value = newValue;
                    if (percentLabel != null)
                    {
                        percentLabel.Text = $"{(int)(Value * 100)}%";
                    }

                    RefreshLayout();

                    OnValueChanged?.Invoke(Value);
                }
            }
        }
        else
        {
            isDragging = false;
        }
    }
}