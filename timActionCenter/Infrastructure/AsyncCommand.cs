using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace timActionCenter.Infrastructure;

public sealed class AsyncCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Action<Exception> _onError;
    private bool _isRunning;

    public AsyncCommand(Func<Task> execute, Action<Exception> onError)
    {
        _execute = execute;
        _onError = onError;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => !_isRunning;

    public async void Execute(object? parameter)
    {
        if (_isRunning)
        {
            return;
        }

        _isRunning = true;
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        try
        {
            await _execute();
        }
        catch (Exception exception)
        {
            _onError(exception);
        }
        finally
        {
            _isRunning = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
