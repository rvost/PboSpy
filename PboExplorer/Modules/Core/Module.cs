using Caliburn.Micro;
using Gemini.Framework;
using PboExplorer.Modules.Core.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace PboExplorer.Modules.Core;

[Export(typeof(IModule))]
public class Module: ModuleBase
{
    public override IEnumerable<IDocument> DefaultDocuments
    {
        get
        {
            yield return IoC.Get<AboutViewModel>();
        }
    }

    public override async Task PostInitializeAsync()
    {
        await Shell.OpenDocumentAsync(IoC.Get<AboutViewModel>());
        Shell.ShowTool(IoC.Get<ExplorerViewModel>());
        Shell.ShowTool(IoC.Get<ConfigViewModel>());
    }
}
