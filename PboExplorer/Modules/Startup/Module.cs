using PboExplorer.Helpers;
using PboExplorer.Modules.About;
using PboExplorer.Modules.ConfigExplorer;
using PboExplorer.Modules.Explorer;
using System.Windows;

namespace PboExplorer.Modules.Startup;

[Export(typeof(IModule))]
public class Module : ModuleBase
{
    private readonly IMainWindow _mainWindow;

    public override IEnumerable<IDocument> DefaultDocuments
    {
        get
        {
            yield return IoC.Get<IAboutInformation>();
        }
    }

    public override IEnumerable<Type> DefaultTools
    {
        get
        {
            yield return typeof(IPboExplorer);
            yield return typeof(IConfigExplorer);
        }
    }

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

    public override Task PostInitializeAsync()
    {
        Shell.ActiveLayoutItem = IoC.Get<IPboExplorer>();
        return Task.CompletedTask;
    }
}
