using Gemini.Modules.Settings;
using PboExplorer.Properties;

namespace PboExplorer.Modules.Startup.ViewModels;

[Export(typeof(ISettingsEditor))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public class ApplicationSettingsViewModel : PropertyChangedBase, ISettingsEditor
{
    private bool _confirmExit;
    private bool _openFullscreen;

    public ApplicationSettingsViewModel()
    {
        ConfirmExit = Settings.Default.ConfirmExit;
    }

    public bool ConfirmExit
    {
        get => _confirmExit;
        set
        {
            _confirmExit = value;
            NotifyOfPropertyChange(nameof(ConfirmExit));
        }
    }

    public bool OpenFullscreen
    {
        get => _openFullscreen;
        set
        {
            _openFullscreen = value;
            NotifyOfPropertyChange(nameof(OpenFullscreen));
        }
    }

    public string SettingsPageName => "General";

    public string SettingsPagePath => "Environment";

    public void ApplyChanges()
    {
        Settings.Default.ConfirmExit = ConfirmExit;
        Settings.Default.OpenFullscreen = OpenFullscreen;
        Settings.Default.Save();
    }
}
