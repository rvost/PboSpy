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

namespace PboExplorer.Modules.PreviewImage.Views
{
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
}
