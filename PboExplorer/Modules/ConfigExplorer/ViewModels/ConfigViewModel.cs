using Gemini.Modules.PropertyGrid;
using PboExplorer.Interfaces;
using PboExplorer.Models;
using PboExplorer.Modules.ConfigExplorer.Services;
using PboExplorer.Modules.ConfigExplorer.Utils;
using PboExplorer.Modules.ConfigExplorer.ViewModels.Search;
using PboExplorer.Modules.Metadata;
using System.Collections.Specialized;
using System.Windows;

namespace PboExplorer.Modules.ConfigExplorer.ViewModels;

[Export(typeof(IConfigExplorer))]
[PartCreationPolicy(CreationPolicy.Shared)]
internal class ConfigViewModel : Tool, IConfigExplorer
{
    private readonly IConfigManager _configManager;
    private readonly ConfigPreviewManager _previewManager;
    private readonly ITreeItemTransformer<Task<IMetadata>> _metadataTransformer;
    private readonly IPropertyGrid _propertyGrid;
    private readonly OneTaskProcessor _procesor = new();
    private readonly ConfigTreeRootViewModel _root = new();

    private ConfigTreeItemViewModel _selectedItem;
    private bool _isStringContained = true;
    private int _countSearchMatches = 0;
    private bool _isProcessing = false;
    private string _statusStringResult = "";
    private string _searchString = "";

    public ConfigTreeItemViewModel SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            NotifyOfPropertyChange(nameof(SelectedItem));
        }
    }

    public bool IsProcessing
    {
        get => _isProcessing;
        protected set
        {
            if (_isProcessing != value)
            {
                _isProcessing = value;
                NotifyOfPropertyChange(nameof(IsProcessing));
            }
        }
    }

    public string SearchString
    {
        get => _searchString;
        set
        {
            if (_searchString != value)
            {
                _searchString = value;
                NotifyOfPropertyChange(nameof(SearchString));
            }
        }
    }

    public string StatusStringResult
    {
        get => _statusStringResult;
        protected set
        {
            if (_statusStringResult != value)
            {
                _statusStringResult = value;
                NotifyOfPropertyChange(nameof(StatusStringResult));
            }
        }
    }

    public bool IsStringContained
    {
        get => _isStringContained;
        set
        {
            if (_isStringContained != value)
            {
                _isStringContained = value;
                NotifyOfPropertyChange(nameof(IsStringContained));
            }
        }
    }

    public int CountSearchMatches
    {
        get => _countSearchMatches;
        protected set
        {
            if (_countSearchMatches != value)
            {
                _countSearchMatches = value;
                NotifyOfPropertyChange(nameof(CountSearchMatches));
            }
        }
    }

    public ConfigTreeRootViewModel Root => _root;

    public bool CanSearch => Root.BackUpRootsCount > 0;

    public override PaneLocation PreferredLocation => PaneLocation.Left;

    [ImportingConstructor]
    public ConfigViewModel(IConfigManager configManager, IPropertyGrid propertyGrid, ConfigPreviewManager previewManager,
         ITreeItemTransformer<Task<IMetadata>> metadataTransformer)
    {
        DisplayName = "Config";

        _configManager = configManager;
        _previewManager = previewManager;
        _metadataTransformer = metadataTransformer;
        _propertyGrid = propertyGrid;
       
        _configManager.Items.CollectionChanged += OnConfigCollectionChanged;
    }

    public async Task OpenPreview(ConfigTreeItemViewModel item)
    {
        if (item != SelectedItem)
        {
            return; // Handle bubbling
        }

        if (item is ConfigTreeItemViewModel classItemVm)
        {
            await _previewManager.ShowPreviewAsync(classItemVm.Model);
        }
    }

    public async void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> args)
    {
        if (args.NewValue is ConfigTreeItemViewModel item)
        {
            SelectedItem = item;
            // TODO: Encapsulate Model
            _propertyGrid.SelectedObject = await item.Model.Reduce(_metadataTransformer);
        }
    }

    public async Task Search(string searchTerm)
    {
        var searchMatch = IsStringContained ? SearchMatch.StringIsContained : SearchMatch.StringIsMatched;
        var param = new SearchParams(searchTerm, searchMatch);

        // TODO: Handle errors
        // Make sure the task always processes the last input but is not started twice
        try
        {
            IsProcessing = true;

            var tokenSource = new CancellationTokenSource();
            var doSearch = new Func<int>(() => Root.DoSearch(param, tokenSource.Token));

            var matchesFound = await _procesor.ExecuteOneTask(doSearch, tokenSource);

            StatusStringResult = searchTerm;
            CountSearchMatches = matchesFound;
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private void OnConfigCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                Root.AddItems(e.NewItems.Cast<ConfigClassItem>());
                break;
            case NotifyCollectionChangedAction.Remove:
                e.NewItems.Cast<ConfigClassItem>().Apply(item => Root.RemoveItem(item));
                break;
            case NotifyCollectionChangedAction.Replace:
                break;
            case NotifyCollectionChangedAction.Move:
                break;
            case NotifyCollectionChangedAction.Reset:
                Root.Clear();
                break;
            default:
                break;
        }
        NotifyOfPropertyChange(nameof(CanSearch));
    }

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
    {
        if (close)
        {
            _configManager.Items.CollectionChanged -= OnConfigCollectionChanged;
            _procesor.Dispose();
        }

        return base.OnDeactivateAsync(close, cancellationToken);
    }
}
