using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PboExplorer.Modules.Core.Commands;

[CommandDefinition]
public class ViewAboutCommandDefinition : CommandDefinition
{
    public const string CommandName = "View.About";

    public override string Name => CommandName;

    public override string Text => "About";

    public override string ToolTip => "About";
}
