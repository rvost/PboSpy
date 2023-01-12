using Gemini.Framework.Menus;
using PboExplorer.Modules.Core.Commands;
using System.ComponentModel.Composition;

namespace PboExplorer.Modules.Core;

public static class MenuDefinitions
{
    // File
    [Export]
    public static readonly MenuItemDefinition FileOpenMenuItem = new TextMenuItemDefinition(
        Gemini.Modules.MainMenu.MenuDefinitions.FileNewOpenMenuGroup, 0, "Open",
        new("pack://application:,,,/Gemini;component/Resources/Icons/Open.png"));

    [Export]
    public static readonly MenuItemGroupDefinition FileOpenCascadeGroup = new(FileOpenMenuItem, 0);

    [Export]
    public static readonly MenuItemDefinition OpenFilesMenuItem = new CommandMenuItemDefinition<OpenFilesCommandDefinition>(
       FileOpenCascadeGroup, 0);

    [Export]
    public static readonly MenuItemDefinition OpenFolderMenuItem = new CommandMenuItemDefinition<OpenFolderCommandDefinition>(
        FileOpenCascadeGroup, 1);

    // Edit
    [Export]
    public static readonly MenuItemGroupDefinition EditMenuGroup = new(
        Gemini.Modules.MainMenu.MenuDefinitions.EditMenu, 0);
    
    [Export]
    public static readonly MenuItemDefinition ExtractCurrentMenuItem = new CommandMenuItemDefinition<ExtractCurrentCommandDefinition>(
       EditMenuGroup, 0);

    [Export]
    public static readonly MenuItemDefinition CopyToClipboardMenuItem = new CommandMenuItemDefinition<CopyToClipboardCommandDefinition>(
       EditMenuGroup, 1);

    [Export]
    public static readonly MenuItemGroupDefinition EditExtractAsMenuGroup = new(
       Gemini.Modules.MainMenu.MenuDefinitions.EditMenu, 0);

    [Export]
    public static readonly MenuItemDefinition ExtractAsTextMenuItem = new CommandMenuItemDefinition<ExtractAsTextCommandDefinition>(
       EditExtractAsMenuGroup, 0);

    [Export]
    public static readonly MenuItemDefinition ExtractAsPngMenuItem = new CommandMenuItemDefinition<ExtractAsPngCommandDefinition>(
      EditExtractAsMenuGroup, 1);

    [Export]
    public static readonly MenuItemGroupDefinition KeysMenuGroup = new(
   Gemini.Modules.MainMenu.MenuDefinitions.EditMenu, 1);

    [Export]
    public static readonly MenuItemDefinition ExtractBiKeyMenuItem = new CommandMenuItemDefinition<ExtractBiKeyCommandDefinition>(
      KeysMenuGroup, 1);


    // View 
    [Export]
    public static readonly MenuItemGroupDefinition ViewMenuGroup = new(
        Gemini.Modules.MainMenu.MenuDefinitions.ViewMenu, 0);

    [Export]
    public static readonly MenuItemDefinition ViewAboutMenuItem = new CommandMenuItemDefinition<ViewAboutCommandDefinition>(
        ViewMenuGroup, 0);

    [Export]
    public static readonly MenuItemGroupDefinition ViewToolsMenuGroup = new(
        Gemini.Modules.MainMenu.MenuDefinitions.ViewMenu, 0);

    [Export]
    public static readonly MenuItemDefinition ViewExplorerMenuItem = new CommandMenuItemDefinition<ViewExplorerCommandDefinition>(
       ViewToolsMenuGroup, 0);

    [Export]
    public static readonly MenuItemDefinition ViewConfigMenuItem = new CommandMenuItemDefinition<ViewConfigCommandDefinition>(
       ViewToolsMenuGroup, 1);
}
