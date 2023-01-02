using Gemini.Framework.Commands;
using PboExplorer.Modules.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PboExplorer.Modules.Core.ViewModels;

// TODO: Move ICommandHandler<CopyToClipboardCommandDefinition> to base class
public class TextPreviewViewModel : PreviewViewModel, ICommandHandler<CopyToClipboardCommandDefinition>
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
}
