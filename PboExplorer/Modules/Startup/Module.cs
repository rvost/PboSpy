using Gemini.Framework;
using Gemini.Framework.Services;
using PboExplorer.Helpers;
using System.ComponentModel.Composition;
using System.Windows;

namespace PboExplorer.Modules.Startup;

[Export(typeof(IModule))]
public class Module : ModuleBase
{
    private readonly IMainWindow _mainWindow;

    [ImportingConstructor]
    public Module(IMainWindow mainWindow)
    {
        _mainWindow = mainWindow;
    }

    public override void Initialize()
    {
        _mainWindow.WindowState = WindowState.Maximized;
        _mainWindow.Title = "PboExplorer";
        _mainWindow.Icon = new IconService().GetIcon("WindowIcon"); // TODO: Fix this

        _mainWindow.Shell.ToolBars.Visible = true;
    }
}
