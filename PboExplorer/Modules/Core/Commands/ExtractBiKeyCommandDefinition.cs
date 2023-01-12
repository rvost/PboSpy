using Gemini.Framework.Commands;
using System;

namespace PboExplorer.Modules.Core.Commands;

[CommandDefinition]
public class ExtractBiKeyCommandDefinition : CommandDefinition
{
    public const string CommandName = "Edit.ExtractBiKey";

    public override string Name => CommandName;

    public override string Text => "Extract .bikey";

    public override string ToolTip => "Extract .bikey from signature";

    public override Uri IconSource
        => new("pack://application:,,,/PboExplorer;component/Resources/Icons/BiKey.png");
}