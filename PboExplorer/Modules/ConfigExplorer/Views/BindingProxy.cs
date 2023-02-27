using System.Windows;

namespace PboExplorer.Modules.ConfigExplorer.Views;

/// <summary>
/// Source: http://www.thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/
/// Issue: http://stackoverflow.com/questions/9994241/mvvm-binding-command-to-contextmenu-item
/// </summary>
public class BindingProxy : Freezable
{
    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));

    public object Data
    {
        get { return GetValue(DataProperty); }
        set { SetValue(DataProperty, value); }
    }

    protected override Freezable CreateInstanceCore()
    {
        return new BindingProxy();
    }
}
