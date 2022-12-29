using Gemini.Framework.Menus;
using PboExplorer.Modules.Core.Commands;
using System.ComponentModel.Composition;

namespace PboExplorer.Modules.Core;

public static class MenuDefinitions
{
    [Export]
    public static readonly MenuItemDefinition FileOpenMenuItem = new TextMenuItemDefinition(
        Gemini.Modules.MainMenu.MenuDefinitions.FileNewOpenMenuGroup, 0, "Open");

    [Export]
    public static readonly MenuItemGroupDefinition FileOpenCascadeGroup = new(FileOpenMenuItem, 0);

    [Export]
    public static readonly MenuItemDefinition OpenFilesMenuItem = new CommandMenuItemDefinition<OpenFilesCommandDefinition>(
       FileOpenCascadeGroup, 0);

    [Export]
    public static readonly MenuItemDefinition OpenFolderMenuItem = new CommandMenuItemDefinition<OpenFolderCommandDefinition>(
        FileOpenCascadeGroup, 1);


    // View 
    [Export]
    public static readonly MenuItemGroupDefinition ViewMenuGroup = new(
        Gemini.Modules.MainMenu.MenuDefinitions.ViewMenu, 0);

    [Export]
    public static readonly MenuItemDefinition ViewAboutMenuItem = new CommandMenuItemDefinition<ViewAboutCommandDefinition>(
        ViewMenuGroup, 0);

    // Exclude default menu options
    [Export]
    public static readonly ExcludeMenuItemDefinition ExcludeOpenMenuItem =
        new(Gemini.Modules.Shell.MenuDefinitions.FileOpenMenuItem);

    [Export]
    public static readonly ExcludeMenuItemDefinition ExcludeNewMenuItem =
       new(Gemini.Modules.Shell.MenuDefinitions.FileNewMenuItem);

    [Export]
    public static readonly ExcludeMenuItemGroupDefinition ExcludeNewMenuGroup =
        new(Gemini.Modules.Shell.MenuDefinitions.FileNewCascadeGroup);
}
