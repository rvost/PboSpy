using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Modules.PropertyGrid;
using PboExplorer.Interfaces;
using PboExplorer.Modules.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace PboExplorer.Modules.Core;

[Export(typeof(IModule))]
public class Module : ModuleBase
{
    private readonly IPropertyGrid _propertyGrid;
    public override IEnumerable<IDocument> DefaultDocuments
    {
        get
        {
            yield return IoC.Get<AboutViewModel>();
        }
    }

    public override IEnumerable<Type> DefaultTools
    {
        get
        {
            yield return typeof(ExplorerViewModel);
            yield return typeof(ConfigViewModel);
        }
    }

    [ImportingConstructor]
    public Module(IPropertyGrid propertyGrid)
    {
        _propertyGrid = propertyGrid;
    }

    public override void Initialize()
    {
        Shell.ActiveDocumentChanged += (sender, e) => RefreshPropertyGrid();
        RefreshPropertyGrid();
    }

    public override async Task PostInitializeAsync()
    {
        Shell.ActiveLayoutItem = IoC.Get<ExplorerViewModel>();
    }

    private void RefreshPropertyGrid()
    {
        _propertyGrid.SelectedObject = Shell.ActiveItem switch
        {
            PreviewViewModel preview => (preview.Model as ITreeItem)?.Metadata, // TODO: Refactor PreviewViewModel
            ConfigClassViewModel config => config.Model?.Metadata,
            _ => null,
        };
    }
}
