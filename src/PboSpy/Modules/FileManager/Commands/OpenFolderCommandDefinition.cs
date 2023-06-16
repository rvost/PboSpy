using System.Windows.Input;

namespace PboSpy.Modules.FileManager.Commands;

[CommandDefinition]
public class OpenFolderCommandDefinition : CommandDefinition
{
    public const string CommandName = "File.Open.Folder";

    public override string Name => CommandName;

    public override string Text => "Folder";

    public override string ToolTip => "Open all files from the folder";

    public override Uri IconSource
        => new("pack://application:,,,/PboSpy;component/Resources/Icons/OpenFolder.png");

    [Export]
    public static CommandKeyboardShortcut KeyGesture =
        new CommandKeyboardShortcut<OpenFolderCommandDefinition>(new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Shift));
}
