using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace ForgeAir.Playout.App.Services;

public class TrayIconService
{
    private TrayIcon _tray;

    public void Initialize()
    {
        _tray = new TrayIcon
        {
            Icon = new WindowIcon("Assets/avalonia-logo.ico"),
            ToolTipText = "ForgeAir"
        };

        // Create menu
        var menu = new NativeMenu();

        var settingsItem = new NativeMenuItem("Settings");
        var settingsSubMenu = new NativeMenu
        {
            new NativeMenuItem("Option 1"),
            new NativeMenuItem("Option 2"),
            new NativeMenuItemSeparator(),
            new NativeMenuItem("Option 3")
        };
        settingsItem.Menu = settingsSubMenu;

        menu.Items.Add(settingsItem);
        _tray.Menu = menu;

        _tray.Clicked += OnTrayClicked;

        // now how tf will i show this pos to tray?
    }

    private void OnTrayClicked(object? sender, EventArgs e)
    {
        Console.WriteLine("Tray icon clicked!");
    }
}