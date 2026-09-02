using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using timActionCenter.Models;
using timActionCenter.Services;
using timActionCenter.ViewModels;

namespace timActionCenter;

public partial class MainWindow : Window
{
    private readonly TileConfigurationStore _configurationStore = new();
    private readonly ApplicationSettingsStore _settingsStore = new();
    private readonly TileCommandRunner _commandRunner = new();
    private readonly StartupService _startupService = new();
    private readonly TrayIconService _trayIcon;

    private ApplicationSettings _settings = new();
    private List<TileConfiguration> _configurations = new();
    private bool _isDialogOpen;
    private bool _isRefreshingState;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        LoadSettings();
        LoadTiles();

        _trayIcon = new TrayIconService();
        _trayIcon.Clicked += OnTrayIconClicked;

        PositionWindow();
        Hide();
    }

    public ObservableCollection<TileViewModel> Tiles { get; } = new();

    private void LoadSettings()
    {
        try
        {
            _settings = _settingsStore.Load();
            _settings.StartWithWindows = _startupService.IsEnabled();
        }
        catch (Exception exception)
        {
            ShowSettingsError(exception);
            _settings = new ApplicationSettings();
        }

        Topmost = _settings.KeepWindowOnTop;
    }

    private void LoadTiles()
    {
        try
        {
            _configurations = _configurationStore.Load();
        }
        catch (Exception exception)
        {
            ShowConfigurationError(exception);
            _configurations = _configurationStore.LoadDefaults();
        }

        RefreshCanvas();
    }

    private void RefreshCanvas()
    {
        Tiles.Clear();

        foreach (TileConfiguration configuration in _configurations.Where(tile => tile.IsVisible))
        {
            Tiles.Add(CreateTile(configuration));
        }
    }

    private TileViewModel CreateTile(TileConfiguration configuration)
    {
        return new TileViewModel(
            configuration,
            _commandRunner,
            ShowTileError);
    }

    private void OnAddTileClick(object sender, RoutedEventArgs eventArgs)
    {
        _isDialogOpen = true;

        try
        {
            var dialog = new AddTileWindow { Owner = this };
            if (dialog.ShowDialog() != true || dialog.Tile is null)
            {
                return;
            }

            _configurations.Add(dialog.Tile);

            try
            {
                _configurationStore.Save(_configurations);
                Tiles.Add(CreateTile(dialog.Tile));
            }
            catch (Exception exception)
            {
                _configurations.Remove(dialog.Tile);
                ShowConfigurationError(exception);
            }
        }
        finally
        {
            _isDialogOpen = false;
        }
    }

    private void OnSettingsClick(object sender, RoutedEventArgs eventArgs)
    {
        _isDialogOpen = true;

        try
        {
            var dialog = new SettingsWindow(_settings, _configurations) { Owner = this };
            if (dialog.ShowDialog() != true ||
                dialog.SavedSettings is null ||
                dialog.SavedTiles is null)
            {
                return;
            }

            try
            {
                _startupService.SetEnabled(dialog.SavedSettings.StartWithWindows);
                _settingsStore.Save(dialog.SavedSettings);
                _configurationStore.Save(dialog.SavedTiles);

                _settings = dialog.SavedSettings;
                _configurations = dialog.SavedTiles;
                Topmost = _settings.KeepWindowOnTop;
                RefreshCanvas();
            }
            catch (Exception exception)
            {
                ShowSettingsError(exception);
            }
        }
        finally
        {
            _isDialogOpen = false;
        }
    }

    private async void OnWindowActivated(object sender, EventArgs eventArgs)
    {
        if (_isRefreshingState)
        {
            return;
        }

        _isRefreshingState = true;

        try
        {
            foreach (TileViewModel tile in Tiles)
            {
                try
                {
                    await tile.RefreshStateAsync();
                }
                catch (Exception exception)
                {
                    Debug.WriteLine($"Could not refresh tile '{tile.Title}': {exception}");
                }
            }
        }
        finally
        {
            _isRefreshingState = false;

            if (_settings.HideOnDeactivation && !IsActive && !_isDialogOpen)
            {
                Hide();
            }
        }
    }

    private void OnWindowDeactivated(object sender, EventArgs eventArgs)
    {
        if (_settings.HideOnDeactivation && !_isDialogOpen && !_isRefreshingState)
        {
            Hide();
        }
    }

    private void OnWindowClosed(object? sender, EventArgs eventArgs)
    {
        _trayIcon.Clicked -= OnTrayIconClicked;
        _trayIcon.Dispose();
    }

    private void OnTrayIconClicked(object? sender, EventArgs eventArgs)
    {
        if (IsVisible)
        {
            return;
        }

        PositionWindow();
        Show();
        Activate();
    }

    private void PositionWindow()
    {
        Rect workArea = SystemParameters.WorkArea;
        Top = workArea.Bottom - Height - 10;
        Left = workArea.Right - Width - 10;
    }

    private void ShowTileError(TileViewModel tile, Exception exception)
    {
        MessageBox.Show(
            this,
            $"{tile.Title} could not be completed.\n\n{exception.Message}",
            "Action Center",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    private void ShowConfigurationError(Exception exception)
    {
        MessageBox.Show(
            this,
            $"The tile configuration could not be saved or loaded.\n\n{exception.Message}",
            "Action Center",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    private void ShowSettingsError(Exception exception)
    {
        MessageBox.Show(
            this,
            $"The application settings could not be saved or loaded.\n\n{exception.Message}",
            "Action Center",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
}
