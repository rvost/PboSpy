using Gemini.Modules.Output;
using Gemini.Modules.StatusBar;
using PboExplorer.Models;
using PboExplorer.Modules.ConfigExplorer.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace PboExplorer.Modules.ConfigExplorer.Services;

[Export]
internal class ConfigPreviewManager
{
    private readonly IOutput _output;
    private readonly IShell _shell;
    private readonly IStatusBar _statusBar;

    [ImportingConstructor]
    public ConfigPreviewManager(IOutput output, IShell shell, IStatusBar statusBar)
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
            _statusBar.Items.Clear();
            _statusBar.AddItem($"Opening preview...", new GridLength(1, GridUnitType.Star));
            _statusBar.AddItem(model.Name, new GridLength(1, GridUnitType.Auto));

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
            _statusBar.Items.Clear();
            Mouse.OverrideCursor = null;
        }
    }

    private void ReportError(string source, Exception ex)
    {
        _output.AppendLine($"ERROR: \"{ex.Message}\" while opening {source}");
        _statusBar.Items.Clear();
        _statusBar.AddItem($"ERROR: {ex.Message}", new GridLength(1, GridUnitType.Star));
        _statusBar.AddItem(source, new GridLength(1, GridUnitType.Auto));
    }
}
