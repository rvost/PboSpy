using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using PboExplorer.Modules.ConfigExplorer.ViewModels.Search;

namespace PboExplorer.Modules.ConfigExplorer.Behaviors;

/// <summary>
/// Implements a <see cref="TextBlock"/> behavior to highlight text as specified by
/// a bound property that adheres to the <see cref="ISelectionRange"/> interface.
/// </summary>
public static class HighlightTextBlockBehavior
{
    private static readonly DependencyProperty RangeProperty =
        DependencyProperty.RegisterAttached("Range",
            typeof(ISelectionRange),
            typeof(HighlightTextBlockBehavior), new PropertyMetadata(null, OnRangeChanged));

    public static ISelectionRange GetRange(DependencyObject obj)
    {
        return (ISelectionRange)obj.GetValue(RangeProperty);
    }

    public static void SetRange(DependencyObject obj, ISelectionRange value)
    {
        obj.SetValue(RangeProperty, value);
    }

    private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var txtblock = d as TextBlock;

        if (txtblock == null)
            return;

        var range = GetRange(d);  // Get the bound Range value to do highlighting

        // Standard background is transparent
        SolidColorBrush normalBackGround = new SolidColorBrush(Color.FromArgb(00, 00, 00, 00));
        if (range != null)
        {
            if (range.NormalBackground != default(Color))
                normalBackGround = new SolidColorBrush(range.NormalBackground);
        }

        // Reset Highlighting - this must be done anyways since
        // multiple selection runs will overlay each other
        var txtrange = new TextRange(txtblock.ContentStart, txtblock.ContentEnd);
        txtrange.ApplyPropertyValue(TextElement.BackgroundProperty, normalBackGround);

        if (range == null)
            return;

        if (range.Start < 0 || range.End < 0) // Nothing to highlight
            return;

        try
        {
            // Standard selection background color
            Color selColor = (range.DarkSkin ? Color.FromArgb(255, 254, 252, 200) : Color.FromArgb(255, 252, 186, 3));

            Brush selectionBackground = new SolidColorBrush(selColor);
            if (range != null)
            {
                if (range.SelectionBackground != default(Color))
                    selectionBackground = new SolidColorBrush(range.SelectionBackground);
            }

            var txtrangel = new TextRange(txtblock.ContentStart.GetPositionAtOffset(range.Start + 1), 
                txtblock.ContentStart.GetPositionAtOffset(range.End + 1));

            txtrangel.ApplyPropertyValue(TextElement.BackgroundProperty, selectionBackground);
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
            Console.WriteLine(exc.StackTrace);
        }
    }
}
