using Gemini.Framework;
using Gemini.Framework.Commands;
using PboExplorer.Modules.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PboExplorer.Modules.Core.ViewModels;

public class TextPreviewViewModel : Document, ICommandHandler<CopyToClipboardCommandDefinition>
{
    public string Text { get; }

	public TextPreviewViewModel(string name, string text)
	{
		DisplayName= name;
		Text= text;
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
