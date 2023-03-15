using PboSpy.Modules.Signatures.Commands;

namespace PboSpy.Modules.Signatures;

internal static class ToolBarDefinitions
{
    [Export]
    public static readonly ToolBarDefinition SignaturesToolBar = new(1, "Signatures");

    [Export]
    public static readonly ToolBarItemGroupDefinition SignaturesToolBarGroup = new(
        SignaturesToolBar, 2);

    [Export]
    public static ToolBarItemDefinition ExtractBiKeyToolBarItem =
       new CommandToolBarItemDefinition<ExtractBiKeyCommandDefinition>(SignaturesToolBarGroup, 3);

    [Export]
    public static ToolBarItemDefinition SaveBiKeyToolBarItem =
       new CommandToolBarItemDefinition<SaveBiKeyCommandDefinition>(SignaturesToolBarGroup, 4);
}
