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
using System.Windows.Threading;

namespace PboSpy.Modules.ConfigExplorer.Views
{
    /// <summary>
    /// Interaction logic for ConfigClassDefinitionView.xaml
    /// </summary>
    public partial class ConfigClassDefinitionView : UserControl, ICodeEditorView
    {
        public ConfigClassDefinitionView()
        {
            InitializeComponent();
        }

        public TextEditor TextEditor => CodeEditor;

        public void ApplySettings()
        {
            CodeEditor?.ApplySettings();
        }

        private void OnExpanded(object sender, RoutedEventArgs e) 
            => Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, () => CodeEditor.Focus());
    }
}
