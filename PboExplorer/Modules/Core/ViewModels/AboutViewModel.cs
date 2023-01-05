using Gemini.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PboExplorer.Modules.Core.ViewModels;

[Export]
public class AboutViewModel : Document
{
    public AboutViewModel()
    {
        DisplayName = "About";
    }

    public override bool ShouldReopenOnStart
    {
        get { return true; }
    }

}
