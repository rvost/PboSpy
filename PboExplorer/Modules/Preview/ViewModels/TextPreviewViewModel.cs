using Gemini.Modules.CodeEditor;
using Gemini.Modules.CodeEditor.Views;
using Microsoft.Win32;
using PboExplorer.Models;
using PboExplorer.Modules.Extraction.Commands;
using System.IO;
using System.Windows;

namespace PboExplorer.Modules.Preview.ViewModels;

public class TextPreviewViewModel : PreviewViewModel, ICommandHandler<ExtractAsTextCommandDefinition>
{

    private readonly LanguageDefinitionManager _languageDefinitionManager;
    private ICodeEditorView _view;

    public string Text { get; }

    public TextPreviewViewModel(FileBase model, string text) : base(model)
    {
        Text = text;

        // TODO: Find better way to load LanguageDefinitionManager
        _languageDefinitionManager = IoC.Get<LanguageDefinitionManager>();
    }

    protected override void CanExecuteCopy(Command command)
    {
        command.Enabled = true;
    }

    protected override Task ExecuteCopy(Command command)
    {
        Clipboard.SetText(Text);
        return Task.CompletedTask;
    }

    protected override void OnViewLoaded(object view)
    {
        _view = (ICodeEditorView)view;
        LoadText();
    }

    private void LoadText()
    {
        if (_view == null)
        {
            return;
        }

        _view.TextEditor.Text = Text;
        _view.TextEditor.IsReadOnly = true;

        var fileExtension = _model.Extension.ToLower();
        if (fileExtension == ".bin")
        {
            fileExtension = ".cpp";
        }

        ILanguageDefinition languageDefinition = _languageDefinitionManager.GetDefinitionByExtension(fileExtension);

        SetLanguage(languageDefinition);
    }

    private void SetLanguage(ILanguageDefinition languageDefinition)
    {
        _view.TextEditor.SyntaxHighlighting = languageDefinition != null
            ? languageDefinition.SyntaxHighlighting
            : null;
    }

    void ICommandHandler<ExtractAsTextCommandDefinition>.Update(Command command)
    {
        command.Enabled = true;
    }

    Task ICommandHandler<ExtractAsTextCommandDefinition>.Run(Command command)
    {
        var dlg = new SaveFileDialog
        {
            Title = "Extract to text file",
            FileName = _model.Extension == ".bin" ? Path.ChangeExtension(_model.Name, ".cpp") : _model.Name,
            DefaultExt = ".txt",
            Filter = "Text file|*.txt|CPP|*.cpp|HPP|*.hpp|SQM|*.sqm|SQF|*.sqf|RVMAT|*.rvmat"
        };

        if (dlg.ShowDialog() == true)
        {
            return File.WriteAllTextAsync(dlg.FileName, Text);
        }

        return Task.CompletedTask;
    }

}
