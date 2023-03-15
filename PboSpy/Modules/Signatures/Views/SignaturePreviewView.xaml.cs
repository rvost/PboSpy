using PboSpy.Modules.Signatures.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PboSpy.Modules.Signatures.Views
{
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
}
