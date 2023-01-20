using Gemini.Modules.Output;
using Gemini.Modules.StatusBar;
using PboExplorer.Models;
using PboExplorer.Modules.Preview.Factories;
using PboExplorer.Modules.Preview.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace PboExplorer.Modules.Preview.Services;

[Export(typeof(IPreviewManager))]
public class PreviewManager : IPreviewManager
{
    private readonly IShell _shell;
    private readonly IStatusBar _statusBar;
    private readonly IOutput _output;
    private readonly PreviewFactory _previewFactory;

    [ImportingConstructor]
    public PreviewManager(IShell shell, IStatusBar statusBar, IOutput output, PreviewFactory previewFactory)
    {
        _shell = shell;
        _statusBar = statusBar;
        _output = output;
        _previewFactory = previewFactory;
    }

    public async Task ShowPreview(FileBase model)
    {
        Document document = _shell.Documents
            .OfType<PreviewViewModel>()
            .Where(doc => doc.Model == model)
            .FirstOrDefault();

        document ??= await CreatePreview(model);

        await _shell.OpenDocumentAsync(document);
    }

    private async Task<Document> CreatePreview(FileBase model)
    {
        try
        {
            _statusBar.Items.Clear();
            _statusBar.AddItem($"Opening preview...", new GridLength(1, GridUnitType.Star));
            _statusBar.AddItem(model.Name, new GridLength(1, GridUnitType.Auto));

            Mouse.OverrideCursor = Cursors.Wait;

            var document = await Task.Run(
                () => _previewFactory.CreatePreview(model)
                );

            return document;
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
