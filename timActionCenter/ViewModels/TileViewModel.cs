using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using timActionCenter.Infrastructure;
using timActionCenter.Models;
using timActionCenter.Presentation;
using timActionCenter.Services;

namespace timActionCenter.ViewModels;

public sealed class TileViewModel : INotifyPropertyChanged
{
    private readonly TileConfiguration _configuration;
    private readonly TileCommandRunner _commandRunner;
    private bool _isActive;

    public TileViewModel(
        TileConfiguration configuration,
        TileCommandRunner commandRunner,
        Action<TileViewModel, Exception> onError)
    {
        _configuration = configuration;
        _commandRunner = commandRunner;
        Command = new AsyncCommand(ExecuteAsync, exception => onError(this, exception));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public Guid Id => _configuration.Id;

    public string Title => _configuration.Title;

    public string Description => _configuration.Description;

    public string IconGlyph => _configuration.IconGlyph;

    public FontFamily IconFontFamily => SegoeFluentIconProvider.FontFamily;

    public ICommand Command { get; }

    public bool IsActive
    {
        get => _isActive;
        private set
        {
            if (_isActive == value)
            {
                return;
            }

            _isActive = value;
            OnPropertyChanged();
        }
    }

    public async Task RefreshStateAsync()
    {
        if (_configuration.Behavior == TileBehavior.Toggle)
        {
            IsActive = await _commandRunner.ReturnsSuccessAsync(_configuration.StateCommand!);
        }
    }

    private async Task ExecuteAsync()
    {
        bool previousState = IsActive;
        bool isToggle = _configuration.Behavior == TileBehavior.Toggle;
        bool newState = isToggle && !previousState;

        if (isToggle)
        {
            IsActive = newState;
        }

        try
        {
            await _commandRunner.RunAsync(GetCommand(newState));
        }
        catch
        {
            IsActive = previousState;
            throw;
        }
    }

    private TileCommand GetCommand(bool newState)
    {
        if (_configuration.Behavior == TileBehavior.Action)
        {
            return _configuration.Command!;
        }

        return newState
            ? _configuration.EnableCommand!
            : _configuration.DisableCommand!;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
