using Gemini.Modules.Output;
using PboExplorer.Models;
using PboExplorer.Modules.ConfigExplorer.ViewModels;
using PboExplorer.Modules.StatusBar;
using System.Windows.Input;

namespace PboExplorer.Modules.ConfigExplorer.Services;

[Export]
internal class ConfigPreviewManager
{
    private readonly IOutput _output;
    private readonly IShell _shell;
    private readonly IStatusBarManager _statusBar;

    [ImportingConstructor]
    public ConfigPreviewManager(IOutput output, IShell shell, IStatusBarManager statusBar)
    {
        _output = output;
        _shell = shell;
        _statusBar = statusBar;
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
        _output.AppendLine($"ERROR: \"{ex.Message}\" while opening {source}");
        _statusBar.SetTemporaryStatus($"ERROR: {ex.Message}. See output for details.", duration: 3000);
    }
}
