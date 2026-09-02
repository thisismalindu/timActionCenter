using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using timActionCenter.Models;
using timActionCenter.Presentation;

namespace timActionCenter;

public partial class AddTileWindow : Window
{
    public AddTileWindow()
    {
        InitializeComponent();
        DataContext = this;

        IconComboBox.SelectedItem = SegoeFluentIconProvider.DefaultIcon;
        BehaviorComboBox.SelectedItem = TileBehavior.Action;
    }

    public IReadOnlyList<IconGlyphOption> IconOptions => SegoeFluentIconProvider.Icons;

    public Array Behaviors { get; } = Enum.GetValues(typeof(TileBehavior));

    public TileConfiguration? Tile { get; private set; }

    private void OnBehaviorChanged(object sender, SelectionChangedEventArgs eventArgs)
    {
        bool isToggle = BehaviorComboBox.SelectedItem is TileBehavior.Toggle;
        ActionPanel.Visibility = isToggle ? Visibility.Collapsed : Visibility.Visible;
        TogglePanel.Visibility = isToggle ? Visibility.Visible : Visibility.Collapsed;
    }

    private void OnAddClick(object sender, RoutedEventArgs eventArgs)
    {
        if (!TryGetBasicDetails(out string title, out string iconGlyph, out TileBehavior behavior))
        {
            return;
        }

        Tile = behavior == TileBehavior.Action
            ? CreateActionTile(title, iconGlyph)
            : CreateToggleTile(title, iconGlyph);

        if (Tile is not null)
        {
            DialogResult = true;
        }
    }

    private bool TryGetBasicDetails(
        out string title,
        out string iconGlyph,
        out TileBehavior behavior)
    {
        title = TitleTextBox.Text.Trim();
        iconGlyph = SegoeFluentIconProvider.DefaultIcon.Glyph;
        behavior = TileBehavior.Action;

        if (string.IsNullOrWhiteSpace(title))
        {
            ShowValidationMessage("Enter a title for the tile.");
            return false;
        }

        if (IconComboBox.SelectedItem is not IconGlyphOption iconOption ||
            BehaviorComboBox.SelectedItem is not TileBehavior selectedBehavior)
        {
            ShowValidationMessage("Select an icon and behavior.");
            return false;
        }

        iconGlyph = iconOption.Glyph;
        behavior = selectedBehavior;
        return true;
    }

    private TileConfiguration? CreateActionTile(string title, string iconGlyph)
    {
        TileCommand? command = CreateCommand(
            CommandTextBox.Text,
            ArgumentsTextBox.Text,
            WorkingDirectoryTextBox.Text,
            WaitForExitCheckBox.IsChecked == true,
            RunHiddenCheckBox.IsChecked == true
                ? CommandWindowMode.Hidden
                : CommandWindowMode.Normal);

        if (command is null)
        {
            return null;
        }

        return CreateTile(title, iconGlyph, TileBehavior.Action, command: command);
    }

    private TileConfiguration? CreateToggleTile(string title, string iconGlyph)
    {
        TileCommand? stateCommand = CreateCommand(StateCommandTextBox.Text, StateArgumentsTextBox.Text);
        TileCommand? enableCommand = CreateCommand(EnableCommandTextBox.Text, EnableArgumentsTextBox.Text);
        TileCommand? disableCommand = CreateCommand(DisableCommandTextBox.Text, DisableArgumentsTextBox.Text);

        if (stateCommand is null || enableCommand is null || disableCommand is null)
        {
            return null;
        }

        stateCommand.WaitForExit = true;
        enableCommand.WaitForExit = true;
        disableCommand.WaitForExit = true;
        stateCommand.WindowMode = CommandWindowMode.Hidden;

        CommandWindowMode toggleWindowMode = RunToggleHiddenCheckBox.IsChecked == true
            ? CommandWindowMode.Hidden
            : CommandWindowMode.Normal;

        enableCommand.WindowMode = toggleWindowMode;
        disableCommand.WindowMode = toggleWindowMode;

        return CreateTile(
            title,
            iconGlyph,
            TileBehavior.Toggle,
            stateCommand: stateCommand,
            enableCommand: enableCommand,
            disableCommand: disableCommand);
    }

    private TileConfiguration CreateTile(
        string title,
        string iconGlyph,
        TileBehavior behavior,
        TileCommand? command = null,
        TileCommand? stateCommand = null,
        TileCommand? enableCommand = null,
        TileCommand? disableCommand = null)
    {
        return new TileConfiguration
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = DescriptionTextBox.Text.Trim(),
            IconGlyph = iconGlyph,
            Behavior = behavior,
            Command = command,
            StateCommand = stateCommand,
            EnableCommand = enableCommand,
            DisableCommand = disableCommand,
        };
    }

    private TileCommand? CreateCommand(
        string fileName,
        string arguments,
        string workingDirectory = "",
        bool waitForExit = false,
        CommandWindowMode windowMode = CommandWindowMode.Normal)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            ShowValidationMessage("Every command needs a command or file path.");
            return null;
        }

        return new TileCommand
        {
            FileName = fileName.Trim(),
            Arguments = arguments.Trim(),
            WorkingDirectory = workingDirectory.Trim(),
            WindowMode = windowMode,
            WaitForExit = waitForExit,
        };
    }

    private void ShowValidationMessage(string message)
    {
        MessageBox.Show(
            this,
            message,
            "Add tile",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}
