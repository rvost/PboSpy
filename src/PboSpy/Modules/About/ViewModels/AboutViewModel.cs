namespace PboSpy.Modules.About.ViewModels;

[Export(typeof(IAboutInformation))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class AboutViewModel : Document, IAboutInformation
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
