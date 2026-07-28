using System.Linq;
using Interfaces;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GraveDigger.GUI.Layouts;

public class VerticalLayout : Layout
{
    
    public enum HorizontalAlignment
    {
        Left,
        Center,
        Right
    } 
    
    public HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Center; 

    public VerticalLayout(Rectangle bounds) : base(bounds)
    {
    }

    public override void UpdateLayout()
    {
        var visibleElements = elements
            .Where(element => element.Visible)
            .ToList();
        
        if (visibleElements.Count == 0)
            return;
        
        int contentHeight = VerticalPadding * (visibleElements.Count - 1);
        foreach (ILayoutElement element in visibleElements)
            contentHeight += (int) element.Size.Y;

        Rectangle contentBounds = GetContentBounds();
        
        int y = (int) (contentBounds.Y + (contentBounds.Height - contentHeight) * 0.5f);
        
        foreach (ILayoutElement element in visibleElements)
        {
            int x = 0;
            switch (Alignment)
            {
                case HorizontalAlignment.Left:
                    x = (int) (contentBounds.Left);
                    break;
                case HorizontalAlignment.Center:
                    x = (int) (contentBounds.Center.X - element.Size.X * 0.5f);
                    break;
                case HorizontalAlignment.Right:
                    x = (int) (contentBounds.Right - element.Size.X);
                    break;
                    
            }
            element.SetPosition(x, y);
            
            y += (int) element.Size.Y + VerticalPadding;
        }
    }
    
    public override void SetPosition(int x, int y)
    {
        base.SetPosition(x, y);
        UpdateLayout();
    }
}