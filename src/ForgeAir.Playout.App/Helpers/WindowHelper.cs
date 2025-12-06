using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ForgeAir.Playout.ViewModels;

namespace ForgeAir.Playout.App.Helpers {

    public static class WindowHelper
    {
        public static Window? GetMainWindow()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                return desktop.MainWindow;

            return null;
        }

        public static Window? GetShellView()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.Windows
                    .FirstOrDefault(w => w.DataContext is ShellViewModel);
            }

            return null;
        }
        public static Window? GetActiveWindow()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                return desktop.Windows.FirstOrDefault(w => w.IsActive);
            
            return null;
        }
    }
}