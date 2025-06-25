using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ForgeAir.Core.Models;
using HandyControl.Controls;

namespace ForgeAir.Playout.TrayIcon
{
    public class ForgeTrayIcon : IDisposable
    {
        System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenu1;
        private System.ComponentModel.IContainer components;

        private AppState _appState;
        public ForgeTrayIcon(AppState appState) {
            _appState = appState;
            trayIcon = new System.Windows.Forms.NotifyIcon();
            trayIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);


            contextMenu1 = new ContextMenuStrip();

            contextMenu1.Items.Add(new ToolStripMenuItem("Unhide Studio", null, new EventHandler(ShowStudio)));
            contextMenu1.Items.Add(new ToolStripMenuItem("Settings", null, new EventHandler(ShowSettings)));
            contextMenu1.Items.Add(new ToolStripSeparator());
            contextMenu1.Items.Add(new ToolStripMenuItem("Quit", null, new EventHandler(Quit_Click), "Quit"));

            trayIcon.ContextMenuStrip = contextMenu1;

            trayIcon.Visible = true;
        }

        void ShowStudio(object? sender, System.EventArgs e)
        {
        }
        void ShowSettings(object? sender, System.EventArgs e) {

        }
        void ShowVSTEditor(object? sender, System.EventArgs e)
        {

        }
        public void Quit_Click(object? sender, System.EventArgs e)
        {
            if (HandyControl.Controls.MessageBox.Show("By exiting ForgeAir, anything that is running in will stop\nContinue?", "Warning", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
