using Gemini.Modules.StatusBar;
using PboExplorer.Models;
using PboExplorer.Modules.ConfigExplorer.ViewModels;
using System.Windows;

namespace PboExplorer.Modules.ConfigExplorer.Services;

[Export]
internal class ConfigPreviewManager
{
    private readonly IShell _shell;
    private readonly IStatusBar _statusBar;

    [ImportingConstructor]
    public ConfigPreviewManager(IShell shell, IStatusBar statusBar)
    {
        _shell = shell;
        _statusBar = statusBar;
    }

    public async Task ShowPreviewAsync(ConfigClassItem model)
    {
        try
        {
            var document = CreatePreview(model);
            await _shell.OpenDocumentAsync(document);

        }
        catch (Exception ex)
        {
            ReportError(model.Name, ex);
        }
    }

    private static Document CreatePreview(ConfigClassItem model)
        => new ConfigClassViewModel(model);

    private void ReportError(string source, Exception ex)
    {
        _statusBar.Items.Clear();
        _statusBar.AddItem($"ERROR: {ex.Message}", new GridLength(1, GridUnitType.Star));
        _statusBar.AddItem(source, new GridLength(1, GridUnitType.Auto));
    }
}
