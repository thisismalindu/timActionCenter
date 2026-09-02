using Microsoft.Win32;
using System;

namespace timActionCenter.Services;

public sealed class StartupService
{
    private const string RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ApplicationName = "timActionCenter";

    public bool IsEnabled()
    {
        using RegistryKey? startupKey = Registry.CurrentUser.OpenSubKey(RegistryPath);
        return startupKey?.GetValue(ApplicationName) is not null;
    }

    public void SetEnabled(bool isEnabled)
    {
        using RegistryKey startupKey = Registry.CurrentUser.CreateSubKey(RegistryPath, writable: true);

        if (!isEnabled)
        {
            startupKey.DeleteValue(ApplicationName, throwOnMissingValue: false);
            return;
        }

        string executablePath = Environment.ProcessPath
            ?? throw new InvalidOperationException("The application path could not be determined.");

        startupKey.SetValue(ApplicationName, $"\"{executablePath}\"", RegistryValueKind.String);
    }
}
