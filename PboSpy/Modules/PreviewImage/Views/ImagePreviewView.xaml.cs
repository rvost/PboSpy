using System.Windows;
using System.Windows.Controls;

namespace PboSpy.Modules.PreviewImage.Views;

/// <summary>
/// Interaction logic for ImageView.xaml
/// </summary>
public partial class ImagePreviewView : UserControl
{
    public ImagePreviewView()
    {
        InitializeComponent();
    }

    public void Reset() => PART_Border.Reset();

    //  TODO: Replace with VM command
    private void OnResetClick(object sender, RoutedEventArgs e)
        => PART_Border.Reset();
}
