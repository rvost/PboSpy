using Gemini.Modules.StatusBar;
using Gemini.Modules.StatusBar.ViewModels;
using System.Windows;

namespace PboSpy.Modules.StatusBar.Services;

[Export(typeof(IStatusBarManager))]
[PartCreationPolicy(CreationPolicy.Shared)]
internal class StatusBarManager : IStatusBarManager
{
    private const string IDLE_STATUS = "Ready";

    private readonly IStatusBar _statusBar;
    private readonly StatusBarItemViewModel _statusItem;
    private readonly StatusBarItemViewModel _detailsItem;

    private CancellationTokenSource _cts;

    [ImportingConstructor]
    public StatusBarManager(IStatusBar statusBar)
    {
        _statusBar = statusBar;
        _statusItem = new StatusBarItemViewModel(IDLE_STATUS, new GridLength(1, GridUnitType.Star));
        _detailsItem = new StatusBarItemViewModel("", new GridLength(1, GridUnitType.Auto));

        _statusBar.Items.Clear();
        _statusBar.Items.Add(_statusItem);
        _statusBar.Items.Add(_detailsItem);
    }

    public void Reset()
    {
        if (_cts is not null)
        {
            _cts.Cancel();
            _cts = null;
        }

        _statusItem.Message = IDLE_STATUS;
        _detailsItem.Message = "";
    }

    public void SetStatus(string message, string details = "")
    {
        if (_cts is not null)
        {
            _cts.Cancel();
            _cts = null;
        }

        _statusItem.Message = message;
        _detailsItem.Message = details;
    }

    public void SetTemporaryStatus(string message, string details = "", int duration = 1500)
    {
        SetStatus(message, details);

        _cts = new CancellationTokenSource();
        Task.Delay(duration, _cts.Token)
            .ContinueWith(t =>
            {
                if (!t.IsCanceled)
                {
                    Reset();
                    _cts = null;
                }
            });
    }
}
