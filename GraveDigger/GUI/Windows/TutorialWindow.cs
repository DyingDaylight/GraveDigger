using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using GraveDigger.Core;
using GraveDigger.Data;
using GraveDigger.GUI.Elements;
using GraveDigger.GUI.Layouts;
using GUI;
using GUI.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.GUI.Windows
{
    public class TutorialWindow : Window
    {
        public event Action OnContractSigned;

        private readonly Image backgroundPaper;
        
        private readonly List<VerticalLayout> pageLayouts = new();
        private readonly List<List<UIElement>> pageElements = new();

        private readonly Button signButton;
        private readonly Button prevButton;
        private readonly Button nextButton;
        private readonly Label pageIndicatorLabel;

        private TutorialContractData contractData;

        private int currentPage = 1;
        private int totalPages = 1;

        private const float MaxTextWidth = 720f;

        private const int ArrowWidth = 84;
        private const int ArrowHeight = 84;

        public TutorialWindow(Rectangle screenBounds) : base(screenBounds)
        {
            Texture = null;

            backgroundPaper = CreateElement<Image>();
            SpriteSheet paperSheet = SpriteManager.GetSprite("TutorialWindow");
            Texture2D paperTex = paperSheet?.Texture ?? GUIResources.ButtonDefaultTexture;
            backgroundPaper.SetImage(paperTex);

            prevButton = CreateElement<Button>(Button.UiButtonMode.Texture);
            nextButton = CreateElement<Button>(Button.UiButtonMode.Texture);

            Texture2D leftArrowTex = SpriteManager.GetSprite("ArrowLeft").Texture;
            Texture2D leftArrowHoverTex = SpriteManager.GetSprite("ArrowLeftHover").Texture;
            
            Texture2D rightArrowTex = SpriteManager.GetSprite("ArrowRight").Texture;
            Texture2D rightArrowHoverTex = SpriteManager.GetSprite("ArrowRightHover").Texture;
            
            prevButton.SetTextures(leftArrowTex, leftArrowHoverTex, leftArrowHoverTex);
            nextButton.SetTextures(rightArrowTex, rightArrowHoverTex, rightArrowHoverTex);

            prevButton.LockSize(ArrowWidth, ArrowHeight);
            nextButton.LockSize(ArrowWidth, ArrowHeight);

            prevButton.OnClick += () => ChangePage(-1);
            nextButton.OnClick += () => ChangePage(1);

            pageIndicatorLabel = CreateElement<Label>();
            pageIndicatorLabel.Color = Color.Black;

            signButton = CreateElement<Button>();
            signButton.OnClick += HandleSignClicked;

            LoadContractData("Content/Data/tutorial.json");
            BuildDocumentUI();
            UpdatePageVisibility();
        }

        private VerticalLayout CreatePageLayout()
        {
            return new VerticalLayout(Bounds)
            {
                Alignment = VerticalLayout.HorizontalAlignment.Center,
                VerticalPadding = 12
            };
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D pixelTex = SpriteManager.GetSprite("pixel").Texture;
            spriteBatch.Draw(pixelTex, new Rectangle(0, 0, 1920, 1080), Color.Black * 0.9f);

            backgroundPaper.Draw(spriteBatch);
            base.Draw(spriteBatch);
        }

        private void LoadContractData(string filePath)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
            string pathToRead = File.Exists(fullPath) ? fullPath : filePath;

            if (File.Exists(pathToRead))
            {
                string jsonContent = File.ReadAllText(pathToRead);
                contractData = JsonSerializer.Deserialize<TutorialContractData>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }

        private void CreateNewPage(out VerticalLayout newLayout, out List<UIElement> newElementsList)
        {
            newLayout = CreatePageLayout();
            newElementsList = new List<UIElement>();
            
            pageLayouts.Add(newLayout);
            pageElements.Add(newElementsList);
        }

        private void BuildDocumentUI()
        {
            if (contractData == null) return;

            SpriteFont font = GUIResources.TutorialFont; 

            CreateNewPage(out var p1Layout, out var p1List);
            AddWrappedLabelToPage(p1Layout, p1List, $"{contractData.DocumentNumber}\n\n{contractData.DocumentTitle}", font);
            AddWrappedLabelToPage(p1Layout, p1List, contractData.Preamble, font);

            if (contractData.Sections.Count > 0)
            {
                var duties = contractData.Sections[0];

                CreateNewPage(out var p2Layout, out var p2List);
                AddWrappedLabelToPage(p2Layout, p2List, duties.Title, font);
                if (duties.Items.Count > 0) AddWrappedLabelToPage(p2Layout, p2List, duties.Items[0], font);

                if (duties.Items.Count > 1)
                {
                    CreateNewPage(out var p3Layout, out var p3List);
                    AddWrappedLabelToPage(p3Layout, p3List, duties.Items[1], font);
                }

                if (duties.Items.Count > 2)
                {
                    CreateNewPage(out var p4Layout, out var p4List);
                    AddWrappedLabelToPage(p4Layout, p4List, duties.Items[2], font);
                }

                if (duties.Items.Count > 3)
                {
                    CreateNewPage(out var p5Layout, out var p5List);
                    AddWrappedLabelToPage(p5Layout, p5List, duties.Items[3], font);
                    if (duties.Items.Count > 4) AddWrappedLabelToPage(p5Layout, p5List, duties.Items[4], font);
                }
            }

            CreateNewPage(out var lastLayout, out var lastList);
            if (contractData.Sections.Count > 1)
            {
                var penalties = contractData.Sections[1];
                AddWrappedLabelToPage(lastLayout, lastList, penalties.Title, font);
                foreach (var item in penalties.Items)
                {
                    AddWrappedLabelToPage(lastLayout, lastList, item, font);
                }
            }

            AddWrappedLabelToPage(lastLayout, lastList, $"{contractData.FooterDeclaration}\n{contractData.TraderSignature}\n{contractData.KeeperSignaturePlaceholder}", font);

            signButton.SetFont(font);
            signButton.SetText(contractData.SignButtonText);
            
            lastLayout.AddElement(signButton);
            lastList.Add(signButton);

            totalPages = pageLayouts.Count;
        }

        private void AddWrappedLabelToPage(VerticalLayout layout, List<UIElement> elementList, string rawText, SpriteFont font)
        {
            Label label = CreateElement<Label>();
            label.Font = font;
            label.Text = WrapText(font, rawText, MaxTextWidth);
            label.Color = Color.Black;
            
            layout.AddElement(label);
            elementList.Add(label);
        }

        private string WrapText(SpriteFont font, string text, float maxLineWidth)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            string[] lines = text.Split('\n');
            StringBuilder resultBuilder = new StringBuilder();

            for (int l = 0; l < lines.Length; l++)
            {
                string[] words = lines[l].Split(' ');
                StringBuilder lineBuilder = new StringBuilder();
                float lineWidth = 0f;
                float spaceWidth = font.MeasureString(" ").X;

                foreach (string word in words)
                {
                    Vector2 wordSize = font.MeasureString(word);

                    if (lineWidth + wordSize.X < maxLineWidth)
                    {
                        lineBuilder.Append(word + " ");
                        lineWidth += wordSize.X + spaceWidth;
                    }
                    else
                    {
                        lineBuilder.Append("\n" + word + " ");
                        lineWidth = wordSize.X + spaceWidth;
                    }
                }

                resultBuilder.Append(lineBuilder.ToString().TrimEnd());
                if (l < lines.Length - 1)
                {
                    resultBuilder.Append("\n");
                }
            }

            return resultBuilder.ToString();
        }

        private void ChangePage(int delta)
        {
            currentPage = Math.Clamp(currentPage + delta, 1, totalPages);
            UpdatePageVisibility();
        }

        private void UpdatePageVisibility()
        {
            for (int i = 0; i < pageElements.Count; i++)
            {
                SetListVisibility(pageElements[i], currentPage == (i + 1));
            }

            prevButton.Visible = (currentPage > 1);
            nextButton.Visible = (currentPage < totalPages);

            pageIndicatorLabel.Text = $"{currentPage} / {totalPages}";
        }

        private void SetListVisibility(List<UIElement> list, bool visible)
        {
            foreach (var element in list)
            {
                element.Visible = visible;
            }
        }

        private void HandleSignClicked()
        {
            OnContractSigned?.Invoke();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            int paperWidth = 1300;
            int paperHeight = 1300;
            int paperX = Bounds.X + (Bounds.Width - paperWidth) / 2;
            int paperY = Bounds.Y + (Bounds.Height - paperHeight) / 2;

            backgroundPaper.SetPosition(paperX, paperY);
            backgroundPaper.SetSize(paperWidth, paperHeight);

            int innerAreaWidth = 720;
            int innerAreaHeight = 620; 

            int textXOffset = 70; 

            int textX = paperX + (paperWidth - innerAreaWidth) / 2 + textXOffset;
            int textY = paperY + (paperHeight - innerAreaHeight) / 2 - 20;

            Rectangle contentBounds = new Rectangle(textX, textY, innerAreaWidth, innerAreaHeight);

            if (currentPage >= 1 && currentPage <= pageLayouts.Count)
            {
                var activeLayout = pageLayouts[currentPage - 1];
                activeLayout.SetBounds(contentBounds);
                activeLayout.SetPosition(textX, textY);
            }

            int leftArrowOffset = -15;    
            int rightArrowOffset = 15;

            prevButton.SetPosition(contentBounds.Left - ArrowWidth + leftArrowOffset, paperY + (paperHeight - ArrowHeight) / 2);
            nextButton.SetPosition(contentBounds.Right + rightArrowOffset, paperY + (paperHeight - ArrowHeight) / 2);

            pageIndicatorLabel.SetPosition(paperX + (paperWidth - 60) / 2, paperY + paperHeight - 320);
        }
    }
}