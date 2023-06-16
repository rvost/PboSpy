using Squirrel;
using System.Windows;

namespace PboSpy;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        SquirrelAwareApp.HandleEvents(
            onInitialInstall: SquirrelConfiguration.OnAppInstall,
            onAppUninstall: SquirrelConfiguration.OnAppUninstall,
            onEveryRun: SquirrelConfiguration.OnAppRun
            );
        await SquirrelConfiguration.UpdateApp();

        base.OnStartup(e);
    }
}
