using Caliburn.Micro;
using Gemini.Framework;
using PboExplorer.Modules.Core.ViewModels;
using System;
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

    public override IEnumerable<Type> DefaultTools
    {
        get
        {
            yield return typeof(ExplorerViewModel);
            yield return typeof(ConfigViewModel);
        }
    }

    public override async Task PostInitializeAsync()
    {
        Shell.ActiveLayoutItem= IoC.Get<ExplorerViewModel>();
    }
}
