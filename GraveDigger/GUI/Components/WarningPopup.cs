using System;
using GraveDigger.Core;
using GraveDigger.GUI.Elements;
using GraveDigger.GUI.Layouts;
using GraveDigger.Systems;
using GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.GUI.Components
{
    public class WarningPopup : UIContainer
    {
        private const float SlideSpeed = 700f;
        private const float BaseIconSize = 100f; 
        
        private readonly Image warningIcon;
        private readonly Label warningLabel;
        private readonly HorizontalLayout layout;
        
        private Vector2 currentPosition;
        private Vector2 targetPosition;
        
        private bool isVisible = false;
        private bool musicTriggered = false;
        private float pulseTimer = 0f;

        public WarningPopup()
        {
            warningIcon = CreateElement<Image>();
            warningLabel = CreateElement<Label>();

            warningIcon.SetSize((int)BaseIconSize, (int)BaseIconSize);

            if (GUIResources.DefaultFont != null)
            {
                warningLabel.Font = GUIResources.DefaultFont;
            }
            warningLabel.Color = Color.Red;

            layout = new HorizontalLayout(Rectangle.Empty)
            {
                HorizontalPadding = 12, 
                Alignment = HorizontalLayout.VerticalAlignment.MiddleCenter
            };

            layout.AddElement(warningIcon);
            layout.AddElement(warningLabel);

            warningIcon.Visible = false;
            warningLabel.Visible = false;
            Visible = false;
        }

        public void Show(string textMessage, Texture2D iconTexture, Rectangle screenBounds, string musicTrackName = "deathmusic")
        {
            if (isVisible)
            {
                if (warningLabel.Text != textMessage)
                {
                    warningLabel.Text = textMessage;
                    UpdateLayoutPosition();
                }
                return;
            }

            isVisible = true;
            Visible = true;
            warningIcon.Visible = true;
            warningLabel.Visible = true;
            pulseTimer = 0f;

            warningIcon.SetImage(iconTexture);
            warningIcon.SetSize((int)BaseIconSize, (int)BaseIconSize); 
    
            warningLabel.Text = textMessage;

            int textWidth = warningLabel.Font != null ? (int)warningLabel.Font.MeasureString(textMessage).X : 300;
            int totalWidth = (int)BaseIconSize + layout.HorizontalPadding + textWidth;
            int totalHeight = Math.Max((int)BaseIconSize, warningLabel.Font != null ? (int)warningLabel.Font.MeasureString(textMessage).Y : 40);

            int targetX = (screenBounds.Width - totalWidth) / 2;
            int targetY = screenBounds.Height - totalHeight - 60;

            currentPosition = new Vector2(targetX, screenBounds.Height + 50);
            targetPosition = new Vector2(targetX, targetY);

            UpdateLayoutPosition();

            if (!musicTriggered && !string.IsNullOrEmpty(musicTrackName))
            {
                AudioManager.Instance.PlayMusic(musicTrackName, loop: true);
                musicTriggered = true;
            }
        }

        public void Hide()
        {
            if (!isVisible) return;

            isVisible = false;
            Visible = false;
            warningIcon.Visible = false;
            warningLabel.Visible = false;
            musicTriggered = false;
            
            AudioManager.Instance.StopMusic();
        }

        public override void Update(GameTime gameTime)
        {
            if (!isVisible) return;

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float distance = Vector2.Distance(currentPosition, targetPosition);
            if (distance > 2f)
            {
                Vector2 direction = targetPosition - currentPosition;
                float step = SlideSpeed * delta;

                if (distance <= step)
                {
                    currentPosition = targetPosition;
                }
                else
                {
                    currentPosition += Vector2.Normalize(direction) * step;
                }
            }

            pulseTimer += delta * 6f; 
            float scale = 1f + 0.12f * (float)Math.Sin(pulseTimer); 
            int animatedIconSize = (int)(BaseIconSize * scale);

            warningIcon.SetSize(animatedIconSize, animatedIconSize);

            UpdateLayoutPosition();

            base.Update(gameTime);
        }

        private void UpdateLayoutPosition()
        {
            SetPosition((int)currentPosition.X, (int)currentPosition.Y);

            int textWidth = warningLabel.Font != null ? (int)warningLabel.Font.MeasureString(warningLabel.Text).X : 300;
            int textHeight = warningLabel.Font != null ? (int)warningLabel.Font.MeasureString(warningLabel.Text).Y : 40;
            
            int totalWidth = (int)warningIcon.Size.X + layout.HorizontalPadding + textWidth;
            int totalHeight = Math.Max((int)warningIcon.Size.Y, textHeight);

            layout.SetBounds(new Rectangle((int)currentPosition.X, (int)currentPosition.Y, totalWidth, totalHeight));
            layout.UpdateLayout();
        }
    }
}