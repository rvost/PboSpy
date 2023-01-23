using Gemini.Modules.Output;
using PboExplorer.Models;
using PboExplorer.Modules.Preview.Factories;
using PboExplorer.Modules.Preview.ViewModels;
using PboExplorer.Modules.StatusBar;
using System.Windows.Input;

namespace PboExplorer.Modules.Preview.Services;

[Export(typeof(IPreviewManager))]
public class PreviewManager : IPreviewManager
{
    private readonly IShell _shell;
    private readonly IStatusBarManager _statusBar;
    private readonly IOutput _output;
    private readonly PreviewFactory _previewFactory;

    [ImportingConstructor]
    public PreviewManager(IShell shell, IStatusBarManager statusBar, IOutput output, PreviewFactory previewFactory)
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
            _statusBar.SetStatus($"Opening preview...", model.Name);
            Mouse.OverrideCursor = Cursors.Wait;

            var document = await Task.Run(
                () => _previewFactory.CreatePreview(model)
                );

            _statusBar.Reset();
           
            return document;
        }
        catch (Exception ex)
        {
            ReportError(model.Name, ex);
            return null;
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    private void ReportError(string source, Exception ex)
    {
        _output.AppendLine($"ERROR: \"{ex.Message}\" while opening {source}");

        _statusBar.SetTemporaryStatus($"ERROR: {ex.Message}. See output for details.", duration: 3000);
    }
}
