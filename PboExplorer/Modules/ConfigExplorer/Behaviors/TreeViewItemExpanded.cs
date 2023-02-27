using PboExplorer.Modules.ConfigExplorer.Utils;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace PboExplorer.Modules.ConfigExplorer.Behaviors;

/// <summary>
/// Attached behaviour to bring a selected TreeViewItem
/// into view when selection is driven by the viewmodel.
/// </summary>
public static class TreeViewItemExpanded
{
    public static ICommand GetCommand(DependencyObject obj)
    {
        return (ICommand)obj.GetValue(CommandProperty);
    }

    public static void SetCommand(DependencyObject obj, ICommand value)
    {
        obj.SetValue(CommandProperty, value);
    }

    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.RegisterAttached("Command",
                                            typeof(ICommand),
                                            typeof(TreeViewItemExpanded),
                                            new PropertyMetadata(null, OnPropertyChanged));
    private static void OnPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
        if (depObj is not TreeViewItem item)
            return;

        if (e.NewValue is not ICommand)
            return;

        if ((ICommand)e.NewValue != null)
        {
            item.Expanded += OnItemExpanded;
        }
        else
        {
            item.Expanded -= OnItemExpanded;
        }
    }

    // TODO: Refactor 
    private static void OnItemExpanded(object sender, RoutedEventArgs e)
    {
        if (sender is not TreeViewItem uiElement)
            return;

        IHasDummyChild f = null;

        if (uiElement.DataContext is IHasDummyChild)
        {
            f = uiElement.DataContext as IHasDummyChild;

            // Message Expand only for those who have 1 dummy folder below
            if (f.HasDummyChild == false)
                return;
        }

        ICommand changedCommand = GetCommand(uiElement);

        if (changedCommand == null || f == null)
            return;

        if (changedCommand is RoutedCommand)
        {
            (changedCommand as RoutedCommand).Execute(f, uiElement);
        }
        else
        {
            changedCommand.Execute(f);
        }
    }
}
