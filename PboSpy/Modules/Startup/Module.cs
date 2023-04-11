using PboSpy.Modules.About;
using PboSpy.Modules.ConfigExplorer;
using PboSpy.Modules.Explorer;
using PboSpy.Modules.FileManager;
using PboSpy.Properties;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PboSpy.Modules.Startup;

[Export(typeof(IModule))]
public class Module : ModuleBase
{
    private readonly IMainWindow _mainWindow;
    private readonly IFileManager _pboManager;

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
    public Module(IMainWindow mainWindow, IFileManager pboManager)
    {
        _mainWindow = mainWindow;
        _pboManager = pboManager;
    }

    public override void Initialize()
    {
        _mainWindow.WindowState = Settings.Default.OpenFullscreen ? WindowState.Maximized : WindowState.Normal;
        _mainWindow.Title = "PboSpy";
        _mainWindow.Icon = new BitmapImage(new("pack://application:,,,/PboSpy;component/Resources/Icons/WindowIcon.png"));

        _mainWindow.Shell.ToolBars.Visible = true;
    }

    public override async Task PostInitializeAsync()
    {
        await LoadFromArguments();

        Shell.ActiveLayoutItem = IoC.Get<IPboExplorer>();
    }

    private async Task LoadFromArguments()
    {
        var args = Environment.GetCommandLineArgs();
        if (args.Length > 1)
        {
            var paths = args
                .Skip(1)
                .Where(str => File.Exists(str) || Directory.Exists(str))
                .ToList();

            if (paths.Any())
            {
                await _pboManager.LoadSupportedFiles(paths);
            }
        }
    }
}
