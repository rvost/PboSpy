using Gemini.Framework.Commands;
using Microsoft.Win32;
using PboExplorer.Modules.Core.Commands;
using PboExplorer.Modules.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PboExplorer.Modules.Core.ViewModels;

// TODO: Move ICommandHandler<CopyToClipboardCommandDefinition> to base class
public class TextPreviewViewModel : PreviewViewModel, ICommandHandler<CopyToClipboardCommandDefinition>,
    ICommandHandler<ExtractAsTextCommandDefinition>
{
    public string Text { get; }

    public TextPreviewViewModel(FileBase model, string text) : base(model)
    {
        Text = text;
    }

    void ICommandHandler<CopyToClipboardCommandDefinition>.Update(Command command)
    {
        command.Enabled = true;
    }

    Task ICommandHandler<CopyToClipboardCommandDefinition>.Run(Command command)
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
