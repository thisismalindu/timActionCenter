using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace timActionCenter.Services;

public sealed class TrayIconService : IDisposable
{
    private readonly NotifyIcon _notifyIcon;

    public TrayIconService()
    {
        string iconPath = Path.Combine(AppContext.BaseDirectory, "settings.ico");

        _notifyIcon = new NotifyIcon
        {
            Icon = new Icon(iconPath),
            Text = "Action Center",
            Visible = true,
        };

        _notifyIcon.Click += OnClick;
    }

    public event EventHandler? Clicked;

    public void Dispose()
    {
        _notifyIcon.Click -= OnClick;
        _notifyIcon.Visible = false;

        Icon? icon = _notifyIcon.Icon;
        _notifyIcon.Icon = null;
        icon?.Dispose();
        _notifyIcon.Dispose();
    }

    private void OnClick(object? sender, EventArgs eventArgs)
    {
        Clicked?.Invoke(this, eventArgs);
    }
}
