using Caliburn.Micro;
using ForgeAir.Core.Models;
using ForgeAir.Playout.ViewModels;
using ForgeAir.Playout.Views;
using ForgeAir.Playout.Views.PlayoutWindows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ForgeAir.Playout.TrayIcon
{
    public class ForgeTrayIcon : IDisposable
    {
        System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenu1;
        private System.ComponentModel.IContainer components;

        private readonly IServiceProvider _provider;
        private readonly IWindowManager _windowManager;
        public ForgeTrayIcon(IServiceProvider provider, IWindowManager windowManager) {
            _provider = provider;
            _windowManager = windowManager;
            trayIcon = new System.Windows.Forms.NotifyIcon();
            trayIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            contextMenu1 = new ContextMenuStrip();
            contextMenu1.Items.Add(new ToolStripMenuItem("Show ForgeAir", null, new EventHandler(ShowStudio)));
            contextMenu1.Items.Add(new ToolStripSeparator());
            contextMenu1.Items.Add(new ToolStripMenuItem("Quit", null, new EventHandler(Quit_Click), "Quit"));
#if DEBUG
            contextMenu1.Items.Add(new ToolStripSeparator());
            contextMenu1.Items.Add(new ToolStripMenuItem("Crash On Purpose", null, new EventHandler(Crash_Click), "Crash On Purpose"));
#endif
            trayIcon.ContextMenuStrip = contextMenu1;

            trayIcon.Visible = true;
            trayIcon.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    typeof(System.Windows.Forms.NotifyIcon).GetMethod("ShowContextMenu", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                        ?.Invoke(trayIcon, null);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    ShowStudio(s, e);
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    ShowSettings(s, e);
                }
            };
        }

        private void Crash_Click(object? sender, EventArgs e)
        {

        }

        void ShowStudio(object? sender, EventArgs e)
        {
            var window = System.Windows.Application.Current.Windows
                .OfType<System.Windows.Window>()
                .FirstOrDefault(w => w.DataContext is ShellViewModel);

            if (window == null)
                return;

            if (!window.IsVisible)
            {
                window.Show();
                window.Activate();

            }

        }
        void ShowSettings(object? sender, System.EventArgs e) {
            _provider.GetRequiredService<ShellViewModel>();
        }
        void ShowVSTEditor(object? sender, System.EventArgs e)
        {

        }
        public void Quit_Click(object? sender, System.EventArgs e)
        {
            if (HandyControl.Controls.MessageBox.Show("By exiting ForgeAir, the playout and any ForgeAir related services will be stopped.\nContinue?", "Quit ForgeAir", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning, System.Windows.MessageBoxResult.No) == System.Windows.MessageBoxResult.Yes)
            {
                this.Dispose(); // safely kill trayicon to avoid staying in the taskbar after exit
                Environment.Exit(0);
            }

        }

        public void Dispose()
        {
            trayIcon.Dispose();
        }

    }
}
