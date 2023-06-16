using System.Windows.Input;

namespace PboSpy.Modules.About.Commands;

[CommandDefinition]
public class ViewAboutCommandDefinition : CommandDefinition
{
    public const string CommandName = "View.About";

    public override string Name => CommandName;

    public override string Text => "About";

    public override string ToolTip => "About";

    public override Uri IconSource
        => new("pack://application:,,,/PboSpy;component/Resources/Icons/AboutBox.png");

    [Export]
    public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<ViewAboutCommandDefinition>(new KeyGesture(Key.F1));
}
