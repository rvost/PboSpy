using System.Windows.Input;

namespace PboExplorer.Modules.Extraction.Commands;

[CommandDefinition]
public class CopyToClipboardCommandDefinition : CommandDefinition
{

    public const string CommandName = "Edit.Copy";

    public override string Name => CommandName;

    public override string Text => "Copy To Clipboard";

    public override string ToolTip => "Copy current file to clipboard";

    public override Uri IconSource
        => new("pack://application:,,,/PboExplorer;component/Resources/Icons/Copy.png");

    [Export]
    public static CommandKeyboardShortcut KeyGesture =
        new CommandKeyboardShortcut<CopyToClipboardCommandDefinition>(new KeyGesture(Key.C, ModifierKeys.Control));
}
