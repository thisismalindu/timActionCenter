using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using timActionCenter.Models;
using timActionCenter.Services;
using timActionCenter.ViewModels;

namespace timActionCenter;

public partial class MainWindow : Window
{
    private readonly TileConfigurationStore _configurationStore = new();
    private readonly TileCommandRunner _commandRunner = new();
    private readonly TrayIconService _trayIcon;

    private List<TileConfiguration> _configurations = new();
    private bool _isDialogOpen;
    private bool _isRefreshingState;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        LoadTiles();

        _trayIcon = new TrayIconService();
        _trayIcon.Clicked += OnTrayIconClicked;

        PositionWindow();
        Hide();
    }

    public ObservableCollection<TileViewModel> Tiles { get; } = new();

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

        foreach (TileConfiguration configuration in _configurations)
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

            if (!IsActive && !_isDialogOpen)
            {
                Hide();
            }
        }
    }

    private void OnWindowDeactivated(object sender, EventArgs eventArgs)
    {
        if (!_isDialogOpen && !_isRefreshingState)
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
}
