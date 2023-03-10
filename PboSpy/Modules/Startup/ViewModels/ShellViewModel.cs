using Gemini.Modules.Shell.Views;
using PboSpy.Properties;
using System.Windows;

namespace PboSpy.Modules.Startup.ViewModels;

[Export(typeof(IShell))]
public class ShellViewModel : Gemini.Modules.Shell.ViewModels.ShellViewModel
{
    static ShellViewModel()
    {
        ViewLocator.AddNamespaceMapping(typeof(ShellViewModel).Namespace, typeof(ShellView).Namespace);
    }

    public override Task<bool> CanCloseAsync(CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>();

        Coroutine.BeginExecute(CanClose().GetEnumerator(), null, (s, e) => tcs.SetResult(!e.WasCancelled));

        return tcs.Task;
    }

    private IEnumerable<IResult> CanClose()
    {
        yield return new MessageBoxResult();
    }

    private class MessageBoxResult : IResult
    {
        public event EventHandler<ResultCompletionEventArgs> Completed;

        public void Execute(CoroutineExecutionContext context)
        {
            var result = System.Windows.MessageBoxResult.Yes;

            if (Settings.Default.ConfirmExit)
            {
                result = MessageBox.Show("Are you sure you want to exit?", "Confirm", 
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
            }

            Completed(this, new ResultCompletionEventArgs { WasCancelled = (result != System.Windows.MessageBoxResult.Yes) });
        }
    }
}
