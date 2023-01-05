using Gemini.Framework.Commands;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace PboExplorer.Modules.Core.Commands;

[CommandDefinition]
public class OpenFilesCommandDefinition : CommandDefinition
{
    public const string CommandName = "File.Open.Files";

    public override string Name => CommandName;

    public override string Text => "Files";

    public override string ToolTip => "Open one or multiple files";

    public override Uri IconSource 
        => new("pack://application:,,,/PboExplorer;component/Resources/Icons/OpenFiles.png");

    [Export]
    public static CommandKeyboardShortcut KeyGesture = 
        new CommandKeyboardShortcut<OpenFilesCommandDefinition>(new KeyGesture(Key.O, ModifierKeys.Control));
}
