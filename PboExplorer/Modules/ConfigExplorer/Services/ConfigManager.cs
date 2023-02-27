using PboExplorer.Interfaces;
using PboExplorer.Models;
using PboExplorer.Modules.PboManager;

namespace PboExplorer.Modules.ConfigExplorer.Services;

[Export(typeof(IConfigManager))]
[PartCreationPolicy(CreationPolicy.Shared)]
internal class ConfigManager : IConfigManager, IDisposable
{
    private readonly IPboManager _pboManager;
    private readonly ConfigClassItem _configRoot;

    public ObservableCollection<ITreeItem> Items { get; }

    [ImportingConstructor]
    public ConfigManager(IPboManager pboManager)
    {
        _pboManager = pboManager;
        _configRoot = new();

        Items = new BindableCollection<ITreeItem>();

        _pboManager.PboLoaded += OnPboLoaded;
        _pboManager.PboRemoved += OnPboRemoved;
    }

    public void Dispose()
    {
        _pboManager.PboLoaded -= OnPboLoaded;
        _pboManager.PboRemoved -= OnPboRemoved;
    }

    private void GenerateMergedConfig(PboFile file)
    {
        var configClasses = _configRoot.MergedView(file);

        // TODO: Refactor ConfigTree update
        Items.Clear();
        foreach (var configClass in configClasses)
        {
            Items.Add(configClass);
        }
    }

    private void OnPboLoaded(object sender, PboManagerEventArgs e)
        => GenerateMergedConfig(e.File);

    // TODO: Remove config entries from removed file
    private void OnPboRemoved(object sender, PboManagerEventArgs e)
    {
        ;
    }
}
