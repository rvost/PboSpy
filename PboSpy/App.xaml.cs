using Squirrel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PboSpy
{
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
}
