using Gemini.Modules.Output;
using Microsoft.Extensions.Logging;
using PboSpy.Modules.ConfigExplorer.Models;
using PboSpy.Modules.ConfigExplorer.ViewModels;
using PboSpy.Modules.StatusBar;
using System.Windows.Input;

namespace PboSpy.Modules.ConfigExplorer.Services;

[Export]
internal class ConfigPreviewManager
{
    private readonly IOutput _output;
    private readonly IShell _shell;
    private readonly IStatusBarManager _statusBar;
    private readonly ILogger<ConfigPreviewManager> _logger;

    [ImportingConstructor]
    public ConfigPreviewManager(IOutput output, IShell shell, IStatusBarManager statusBar,
        ILoggerFactory loggerFactory)
    {
        _output = output;
        _shell = shell;
        _statusBar = statusBar;
        _logger = loggerFactory.CreateLogger<ConfigPreviewManager>();
    }

    public async Task ShowPreviewAsync(ConfigClassItem model)
    {
        var document = CreatePreview(model);
        await _shell.OpenDocumentAsync(document);
    }

    private Document CreatePreview(ConfigClassItem model)
    {
        try
        {
            _statusBar.SetStatus($"Opening preview...", model.Name);
            Mouse.OverrideCursor = Cursors.Wait;

            return new ConfigClassViewModel(model);
        }
        catch (Exception ex)
        {
            ReportError(model.Name, ex);
            return null;
        }
        finally
        {
            _statusBar.Reset();
            Mouse.OverrideCursor = null;
        }
    }

    private void ReportError(string source, Exception ex)
    {
        _output.AppendLine($"ERROR: \"{ex.Message}\" opening {source}");
        _statusBar.SetTemporaryStatus($"ERROR: {ex.Message}. See output for details.", duration: 3000);
        _logger.LogError(ex, "Error opening Config Preview for {source}", source);
    }
}
