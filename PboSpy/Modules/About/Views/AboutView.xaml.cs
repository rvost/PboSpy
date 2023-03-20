using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PboSpy.Modules.About.Views;

/// <summary>
/// Interaction logic for AboutView.xaml
/// </summary>
public partial class AboutView : UserControl
{
    public AboutView()
    {
        InitializeComponent();
    }

    private void OnHyperLinkRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        e.Handled = true;
    }

}
