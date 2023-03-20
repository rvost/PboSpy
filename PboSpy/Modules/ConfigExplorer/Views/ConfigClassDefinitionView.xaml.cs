using Gemini.Modules.CodeEditor.Views;
using ICSharpCode.AvalonEdit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PboSpy.Modules.ConfigExplorer.Views;

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
