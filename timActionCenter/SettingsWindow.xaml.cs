using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using timActionCenter.Models;

namespace timActionCenter;

public partial class SettingsWindow : Window
{
    public SettingsWindow(
        ApplicationSettings settings,
        IEnumerable<TileConfiguration> tiles)
    {
        InitializeComponent();

        Tiles = new ObservableCollection<TileConfiguration>(tiles.Select(tile => tile.Clone()));
        DataContext = this;

        StartWithWindowsCheckBox.IsChecked = settings.StartWithWindows;
        HideOnDeactivationCheckBox.IsChecked = settings.HideOnDeactivation;
        KeepWindowOnTopCheckBox.IsChecked = settings.KeepWindowOnTop;
        ConfirmDeletionCheckBox.IsChecked = settings.ConfirmBeforeDeletingTiles;
    }

    public ObservableCollection<TileConfiguration> Tiles { get; }

    public ApplicationSettings? SavedSettings { get; private set; }

    public List<TileConfiguration>? SavedTiles { get; private set; }

    private void OnMoveUpClick(object sender, RoutedEventArgs eventArgs)
    {
        MoveTile(sender, offset: -1);
    }

    private void OnMoveDownClick(object sender, RoutedEventArgs eventArgs)
    {
        MoveTile(sender, offset: 1);
    }

    private void MoveTile(object sender, int offset)
    {
        if (sender is not Button { DataContext: TileConfiguration tile })
        {
            return;
        }

        int currentIndex = Tiles.IndexOf(tile);
        int newIndex = currentIndex + offset;

        if (newIndex >= 0 && newIndex < Tiles.Count)
        {
            Tiles.Move(currentIndex, newIndex);
        }
    }

    private void OnDeleteClick(object sender, RoutedEventArgs eventArgs)
    {
        if (sender is not Button { DataContext: TileConfiguration tile })
        {
            return;
        }

        if (ConfirmDeletionCheckBox.IsChecked == true)
        {
            MessageBoxResult result = MessageBox.Show(
                this,
                $"Delete '{tile.Title}'? This cannot be undone after you save settings.",
                "Delete tile",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }
        }

        Tiles.Remove(tile);
    }

    private void OnSaveClick(object sender, RoutedEventArgs eventArgs)
    {
        SavedSettings = new ApplicationSettings
        {
            StartWithWindows = StartWithWindowsCheckBox.IsChecked == true,
            HideOnDeactivation = HideOnDeactivationCheckBox.IsChecked == true,
            KeepWindowOnTop = KeepWindowOnTopCheckBox.IsChecked == true,
            ConfirmBeforeDeletingTiles = ConfirmDeletionCheckBox.IsChecked == true,
        };

        SavedTiles = Tiles.Select(tile => tile.Clone()).ToList();
        DialogResult = true;
    }
}
