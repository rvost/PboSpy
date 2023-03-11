using Microsoft.Extensions.Logging;
using Serilog;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace PboSpy;

public class SingleFileBootstrapper : Gemini.AppBootstrapper
{
    private ILoggerFactory _loggerfactory;
    private Microsoft.Extensions.Logging.ILogger _logger;

    public override bool IsPublishSingleFileHandled => true;

    protected override IEnumerable<Assembly> PublishSingleFileBypassAssemblies
    {
        get
        {
            yield return Assembly.GetAssembly(typeof(Gemini.AppBootstrapper));
            yield return Assembly.GetAssembly(typeof(Gemini.Modules.CodeEditor.ILanguageDefinition));
            //yield return Assembly.GetAssembly(typeof(Gemini.Modules.Inspector.IInspectorTool));
            yield return Assembly.GetAssembly(typeof(Gemini.Modules.Output.IOutput));
            yield return Assembly.GetAssembly(typeof(Gemini.Modules.PropertyGrid.IPropertyGrid));
        }
    }

    protected override void PreInitialize()
    {
        base.PreInitialize();

        _loggerfactory = LoggerFactory.Create(builder =>
        {
            var configuration = new LoggerConfiguration()
                .WriteTo.File("PboSpy_Log_.txt", rollingInterval: RollingInterval.Day);

            builder.AddSerilog(configuration.CreateLogger());
        });

        _logger = _loggerfactory.CreateLogger("Application");
    }

    protected override void BindServices(CompositionBatch batch)
    {
        batch.AddExportedValue<ILoggerFactory>(_loggerfactory);
        
        base.BindServices(batch);
    }

    protected override void OnExit(object sender, EventArgs e)
    {
        _logger.LogInformation("Exiting");

        base.OnExit(sender, e);
    }

    protected override void OnStartup(object sender, StartupEventArgs e)
    {
        _logger.LogInformation("Application started");
        base.OnStartup(sender, e);
    }

    protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        _logger.LogCritical(e.Exception, "Unhandled exception");

        base.OnUnhandledException(sender, e);
    }
}