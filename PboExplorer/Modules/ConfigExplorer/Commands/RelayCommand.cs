using System.Diagnostics;
using System.Windows.Input;

namespace PboExplorer.Modules.ConfigExplorer.Commands;

/// <summary>
/// Source: http://www.codeproject.com/Articles/31837/Creating-an-Internationalized-Wizard-in-WPF
/// </summary>
public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute = null;
    private readonly Predicate<T> _canExecute = null;

    public RelayCommand(Action<T> execute): this(execute, null) { }

    public RelayCommand(Action<T> execute, Predicate<T> canExecute)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged
    {
        add
        {
            if (_canExecute != null)
                CommandManager.RequerySuggested += value;
        }

        remove
        {
            if (_canExecute != null)
                CommandManager.RequerySuggested -= value;
        }
    }
   
    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
        return _canExecute == null ? true : _canExecute((T)parameter);
    }

    public void Execute(object parameter)
    {
        _execute((T)parameter);
    }
}
