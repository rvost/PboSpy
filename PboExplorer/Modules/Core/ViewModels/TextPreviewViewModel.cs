using Gemini.Framework.Commands;
using Microsoft.Win32;
using PboExplorer.Modules.Core.Commands;
using PboExplorer.Modules.Core.Models;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace PboExplorer.Modules.Core.ViewModels;

public class TextPreviewViewModel : PreviewViewModel, ICommandHandler<ExtractAsTextCommandDefinition>
{
    public string Text { get; }

    public TextPreviewViewModel(FileBase model, string text) : base(model)
    {
        Text = text;
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
