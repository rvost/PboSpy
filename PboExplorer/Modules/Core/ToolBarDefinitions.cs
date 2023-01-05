using Gemini.Framework.ToolBars;
using PboExplorer.Modules.Core.Commands;
using System.ComponentModel.Composition;

namespace PboExplorer.Modules.Core;

public static class ToolBarDefinitions
{
    [Export]
    public static readonly ToolBarDefinition CoreToolBar = new(0, "Core");

    [Export]
    public static readonly ToolBarItemGroupDefinition OpenToolBarGroup = new ToolBarItemGroupDefinition(
        CoreToolBar, 0);
    [Export]
    public static ToolBarItemDefinition OpenFilesToolBarItem =
        new CommandToolBarItemDefinition<OpenFilesCommandDefinition>(OpenToolBarGroup, 0);
    [Export]
    public static ToolBarItemDefinition OpenFolderToolBarItem =
        new CommandToolBarItemDefinition<OpenFolderCommandDefinition>(OpenToolBarGroup, 1);

    [Export]
    public static readonly ToolBarItemGroupDefinition CopyPasteToolBarGroup = new ToolBarItemGroupDefinition(
       CoreToolBar, 1);
    [Export]
    public static ToolBarItemDefinition CopyToolBarItem =
       new CommandToolBarItemDefinition<CopyToClipboardCommandDefinition>(CopyPasteToolBarGroup, 0);

    [Export]
    public static readonly ToolBarItemGroupDefinition ExtractToolBarGroup = new ToolBarItemGroupDefinition(
        CoreToolBar, 2);
    [Export]
    public static ToolBarItemDefinition ExtractCurrentToolBarItem = 
        new CommandToolBarItemDefinition<ExtractCurrentCommandDefinition>(ExtractToolBarGroup, 0);
    [Export]
    public static ToolBarItemDefinition ExtractAsTextToolBarItem =
        new CommandToolBarItemDefinition<ExtractAsTextCommandDefinition>(ExtractToolBarGroup, 1);
    [Export]
    public static ToolBarItemDefinition ExtractAsPngToolBarItem =
        new CommandToolBarItemDefinition<ExtractAsPngCommandDefinition>(ExtractToolBarGroup, 2);
}
