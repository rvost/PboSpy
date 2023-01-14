using Gemini.Modules.StatusBar;
using PboExplorer.Models;
using PboExplorer.Modules.Preview.Factories;
using PboExplorer.Modules.Preview.ViewModels;
using System.Windows;

namespace PboExplorer.Modules.Preview.Services;

[Export(typeof(IPreviewManager))]
public class PreviewManager : IPreviewManager
{
    private readonly IShell _shell;
    private readonly IStatusBar _statusBar;
    private readonly PreviewFactory _previewFactory;

    [ImportingConstructor]
    public PreviewManager(IShell shell, IStatusBar statusBar, PreviewFactory previewFactory)
    {
        _shell = shell;
        _statusBar = statusBar;
        _previewFactory = previewFactory;
    }

    public async Task ShowPreview(FileBase model)
    {
        Document document = _shell.Documents
            .OfType<PreviewViewModel>()
            .Where(doc => doc.Model == model)
            .FirstOrDefault();

        try
        {
            document ??= _previewFactory.CreatePreview(model);
            await _shell.OpenDocumentAsync(document);
        }
        catch (Exception ex)
        {
            ReportError(model.Name, ex);
        }
    }

    private void ReportError(string source, Exception ex)
    {
        _statusBar.Items.Clear();
        _statusBar.AddItem($"ERROR: {ex.Message}", new GridLength(1, GridUnitType.Star));
        _statusBar.AddItem(source, new GridLength(1, GridUnitType.Auto));
    }
}
