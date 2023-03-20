using Gemini.Modules.CodeEditor.Views;
using ICSharpCode.AvalonEdit;
using System.Windows.Controls;
using System.Windows.Input;

namespace PboSpy.Modules.Preview.Views;

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
