using Gemini.Framework.Commands;
using Gemini.Framework.Menus;
using Gemini.Modules.Shell.Commands;
using System.ComponentModel.Composition;

namespace PboExplorer.Modules.Startup;

public static class MenuDefinitions
{
    // Exclude default menu options

    // File
    [Export]
    public static readonly ExcludeMenuItemDefinition ExcludeOpenMenuItem =
        new(Gemini.Modules.Shell.MenuDefinitions.FileOpenMenuItem);

    [Export]
    public static readonly ExcludeMenuItemDefinition ExcludeNewMenuItem =
       new(Gemini.Modules.Shell.MenuDefinitions.FileNewMenuItem);

    [Export]
    public static readonly ExcludeMenuItemGroupDefinition ExcludeNewMenuGroup =
        new(Gemini.Modules.Shell.MenuDefinitions.FileNewCascadeGroup);

    [Export]
    public static readonly ExcludeMenuItemDefinition FileSaveMenuItem =
       new(Gemini.Modules.Shell.MenuDefinitions.FileSaveMenuItem);

    [Export]
    public static readonly ExcludeMenuItemDefinition FileSaveAsMenuItem =
       new(Gemini.Modules.Shell.MenuDefinitions.FileSaveAsMenuItem);

    [Export]
    public static readonly ExcludeMenuItemDefinition FileSaveAllMenuItem =
   new(Gemini.Modules.Shell.MenuDefinitions.FileSaveAllMenuItem);

    // Edit
    [Export]
    public static readonly ExcludeMenuItemDefinition EditUndoMenuItem =
       new(Gemini.Modules.UndoRedo.MenuDefinitions.EditUndoMenuItem);

    [Export]
    public static readonly ExcludeMenuItemDefinition EditRedoMenuItem =
       new(Gemini.Modules.UndoRedo.MenuDefinitions.EditRedoMenuItem);

    // View
    [Export]
    public static readonly ExcludeMenuItemDefinition ViewHistoryMenuItem =
       new(Gemini.Modules.UndoRedo.MenuDefinitions.ViewHistoryMenuItem);

    [Export]
    public static readonly ExcludeMenuItemDefinition ViewToolboxMenuItem =
       new(Gemini.Modules.Toolbox.MenuDefinitions.ViewToolboxMenuItem);
}
