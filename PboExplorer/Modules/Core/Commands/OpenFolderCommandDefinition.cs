using Gemini.Framework.Commands;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace PboExplorer.Modules.Core.Commands;

[CommandDefinition]
public class OpenFolderCommandDefinition : CommandDefinition
{
    public const string CommandName = "File.Open.Folder";

    public override string Name => CommandName;

    public override string Text => "Folder...";

    public override string ToolTip => "Open all files from the folder";

    [Export]
    public static CommandKeyboardShortcut KeyGesture =
        new CommandKeyboardShortcut<OpenFolderCommandDefinition>(new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Shift));
}
