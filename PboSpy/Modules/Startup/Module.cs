﻿using PboSpy.Helpers;
using PboSpy.Modules.About;
using PboSpy.Modules.ConfigExplorer;
using PboSpy.Modules.Explorer;
using PboSpy.Properties;
using System.Windows;

namespace PboSpy.Modules.Startup;

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
        _mainWindow.WindowState = Settings.Default.OpenFullscreen ? WindowState.Maximized : WindowState.Normal;
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