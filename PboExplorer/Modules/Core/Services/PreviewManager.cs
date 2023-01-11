using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.StatusBar;
using PboExplorer.Modules.Core.Factories;
using PboExplorer.Modules.Core.Models;
using PboExplorer.Modules.Core.ViewModels;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PboExplorer.Modules.Core.Services;

[Export(typeof(IPreviewManager))]
public class PreviewManager : IPreviewManager
{
    private readonly IShell _shell;
    private readonly IStatusBar _statusBar;

    [ImportingConstructor]
    public PreviewManager(IShell shell, IStatusBar statusBar)
    {
        _shell = shell;
        _statusBar = statusBar;
    }

    public async Task ShowPreviewAsync(FileBase model)
    {
        Document document = _shell.Documents
            .OfType<PreviewViewModel>()
            .Where(doc => doc.Model == model)
            .FirstOrDefault();

        try
        {
            document ??= DocumentFactory.CreatePreview(model);
            await _shell.OpenDocumentAsync(document);
        }
        catch (Exception ex)
        {
            ReportError(model.Name, ex);
        }
    }

    public async Task ShowPreviewAsync(ConfigClassItem model)
    {
        try
        {
            var document = DocumentFactory.CreatePreview(model);
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
