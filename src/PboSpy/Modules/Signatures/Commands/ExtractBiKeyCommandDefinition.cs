﻿namespace PboSpy.Modules.Signatures.Commands;

[CommandDefinition]
public class ExtractBiKeyCommandDefinition : CommandDefinition
{
    public const string CommandName = "Edit.ExtractBiKey";

    public override string Name => CommandName;

    public override string Text => "Extract .bikey";

    public override string ToolTip => "Extract .bikey from signature";

    public override Uri IconSource
        => new("pack://application:,,,/PboSpy;component/Resources/Icons/ExtractKey.png");
}
