using PboExplorer.Modules.Extraction.Commands;

namespace PboExplorer.Modules.Extraction;

internal static class ToolBarDefinitions
{
    [Export]
    public static readonly ToolBarDefinition ExtractionToolBar = new(0, "Extraction");

    [Export]
    public static readonly ToolBarItemGroupDefinition CopyPasteToolBarGroup =
        new(ExtractionToolBar, 1);
    [Export]
    public static ToolBarItemDefinition CopyToolBarItem =
       new CommandToolBarItemDefinition<CopyToClipboardCommandDefinition>(CopyPasteToolBarGroup, 0);

    [Export]
    public static readonly ToolBarItemGroupDefinition ExtractToolBarGroup =
        new(ExtractionToolBar, 2);

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
