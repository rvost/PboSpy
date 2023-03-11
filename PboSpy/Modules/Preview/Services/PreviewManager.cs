using Gemini.Modules.Output;
using Microsoft.Extensions.Logging;
using PboSpy.Models;
using PboSpy.Modules.Preview.Factories;
using PboSpy.Modules.Preview.ViewModels;
using PboSpy.Modules.StatusBar;
using System.Windows.Input;

namespace PboSpy.Modules.Preview.Services;

[Export(typeof(IPreviewManager))]
public class PreviewManager : IPreviewManager
{
    private readonly IShell _shell;
    private readonly IStatusBarManager _statusBar;
    private readonly IOutput _output;
    private readonly PreviewFactory _previewFactory;
    private readonly ILogger<PreviewManager> _logger;

    [ImportingConstructor]
    public PreviewManager(IShell shell, IStatusBarManager statusBar, IOutput output, PreviewFactory previewFactory,
        ILoggerFactory loggerFactory)
    {
        _shell = shell;
        _statusBar = statusBar;
        _output = output;
        _previewFactory = previewFactory;
        _logger = loggerFactory.CreateLogger<PreviewManager>();
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
        _logger.LogError(ex, "Error opening File Preview for {source}", source);
    }
}
