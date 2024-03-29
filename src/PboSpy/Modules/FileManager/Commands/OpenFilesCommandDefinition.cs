﻿using System.Windows.Input;

namespace PboSpy.Modules.FileManager.Commands;

[CommandDefinition]
public class OpenFilesCommandDefinition : CommandDefinition
{
    public const string CommandName = "File.Open.Files";

    public override string Name => CommandName;

    public override string Text => "Files";

    public override string ToolTip => "Open one or multiple files";

    public override Uri IconSource
        => new("pack://application:,,,/PboSpy;component/Resources/Icons/OpenFiles.png");

    [Export]
    public static CommandKeyboardShortcut KeyGesture =
        new CommandKeyboardShortcut<OpenFilesCommandDefinition>(new KeyGesture(Key.O, ModifierKeys.Control));
}
