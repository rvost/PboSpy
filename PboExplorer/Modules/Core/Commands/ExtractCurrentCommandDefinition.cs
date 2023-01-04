using Gemini.Framework.Commands;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace PboExplorer.Modules.Core.Commands;

[CommandDefinition]
public class ExtractCurrentCommandDefinition: CommandDefinition
{
    public const string CommandName = "Edit.Extract";

    public override string Name => CommandName;

    public override string Text => "Extract";

    public override string ToolTip => "Extract current file";

    public override Uri IconSource
        => new("pack://application:,,,/PboExplorer;component/Resources/Icons/ExtractFile.png");

    [Export]
    public static CommandKeyboardShortcut KeyGesture =
        new CommandKeyboardShortcut<ExtractCurrentCommandDefinition>(new KeyGesture(Key.E, ModifierKeys.Control));
}
