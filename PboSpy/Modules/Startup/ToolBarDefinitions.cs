namespace PboSpy.Modules.Startup;

internal static class ToolBarDefinitions
{
    // Exclude default toolbar items

    [Export]
    public static readonly ExcludeToolBarItemGroupDefinition StandardOpenSaveToolBarGroup =
       new(Gemini.Modules.Shell.ToolBarDefinitions.StandardOpenSaveToolBarGroup);

    [Export]
    public static readonly ExcludeToolBarItemGroupDefinition StandardUndoRedoToolBarGroup =
       new(Gemini.Modules.UndoRedo.ToolBarDefinitions.StandardUndoRedoToolBarGroup);

    [Export]
    public static readonly ExcludeToolBarItemGroupDefinition CodeEditorToolBarGroup =
       new(Gemini.Modules.CodeEditor.CodeEditorToolBarDefinitions.CodeEditorToolBarGroup);
}
