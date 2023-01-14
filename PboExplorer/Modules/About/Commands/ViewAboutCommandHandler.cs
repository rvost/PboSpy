using Gemini.Framework.Results;

namespace PboExplorer.Modules.About.Commands;

[CommandHandler]
public class ViewAboutCommandHandler : CommandHandlerBase<ViewAboutCommandDefinition>
{
    public override async Task Run(Command command)
    {
        await Show.Document<IAboutInformation>().ExecuteAsync();
    }
}
