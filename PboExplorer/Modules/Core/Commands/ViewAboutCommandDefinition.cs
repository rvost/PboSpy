using Gemini.Framework.Commands;
using System;

namespace PboExplorer.Modules.Core.Commands;

[CommandDefinition]
public class ViewAboutCommandDefinition : CommandDefinition
{
    public const string CommandName = "View.About";

    public override string Name => CommandName;

    public override string Text => "About";

    public override string ToolTip => "About";

    public override Uri IconSource
        => new("pack://application:,,,/PboExplorer;component/Resources/Icons/AboutBox.png");
}
