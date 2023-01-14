using Gemini.Modules.CodeEditor.Controls;
using Gemini.Modules.CodeEditor.Views;
using ICSharpCode.AvalonEdit;
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

namespace PboExplorer.Modules.Preview.Views
{
    /// <summary>
    /// Interaction logic for TextPreviewView.xaml
    /// </summary>
    public partial class TextPreviewView : UserControl, ICodeEditorView
    {
        public TextPreviewView()
        {
            InitializeComponent();
            Loaded += (sender, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        public TextEditor TextEditor => this.CodeEditor;

        public void ApplySettings()
        {
            this.CodeEditor?.ApplySettings();
        }
    }
}
