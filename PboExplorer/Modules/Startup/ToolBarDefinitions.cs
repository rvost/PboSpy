using Gemini.Framework.ToolBars;
using System.ComponentModel.Composition;

namespace PboExplorer.Modules.Startup;

public static class ToolBarDefinitions
{
    // Exclude default toolbar items

    [Export]
    public static readonly ExcludeToolBarItemGroupDefinition StandardOpenSaveToolBarGroup =
       new(Gemini.Modules.Shell.ToolBarDefinitions.StandardOpenSaveToolBarGroup);

    [Export]
    public static readonly ExcludeToolBarItemGroupDefinition StandardUndoRedoToolBarGroup =
       new(Gemini.Modules.UndoRedo.ToolBarDefinitions.StandardUndoRedoToolBarGroup);
}
