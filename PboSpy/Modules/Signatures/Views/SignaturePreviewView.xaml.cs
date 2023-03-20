using PboSpy.Modules.Signatures.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace PboSpy.Modules.Signatures.Views;

/// <summary>
/// Interaction logic for SignaturePreviewView.xaml
/// </summary>
public partial class SignaturePreviewView : UserControl, IHexEditorView
{
    public SignaturePreviewView()
    {
        InitializeComponent();
    }

    public void SubmitChanges() => HexEditor.SubmitChanges();
    public void SubmitChanges(string path) => HexEditor.SubmitChanges(path, true);

    // TODO: Use beheviour instead
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is SignaturePreviewViewModel vm)
        {
            HexEditor.CustomBackgroundBlockItems = vm.Highlighting;
        }
    }
}
