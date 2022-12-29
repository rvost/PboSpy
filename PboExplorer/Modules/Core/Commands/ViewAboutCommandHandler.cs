using Caliburn.Micro;
using Gemini.Framework.Commands;
using Gemini.Framework.Menus;
using Gemini.Framework.Results;
using PboExplorer.Modules.Core.ViewModels;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace PboExplorer.Modules.Core.Commands;

[CommandHandler]
public class ViewAboutCommandHandler : CommandHandlerBase<ViewAboutCommandDefinition>
{
    public override async Task Run(Command command)
    {
        await Show.Document<AboutViewModel>().ExecuteAsync();
    }
}
