using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ForgeAir.Core.Shared;
using HandyControl.Controls;

namespace ForgeAir.Playout.TrayIcon
{
    class ForgeTrayIcon : IDisposable
    {
        System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenu1;
        private System.ComponentModel.IContainer components;
        public ForgeTrayIcon() {
            trayIcon = new System.Windows.Forms.NotifyIcon();
            trayIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);


            contextMenu1 = new ContextMenuStrip();

            contextMenu1.Items.Add(new ToolStripMenuItem("Unhide Studio", null, new EventHandler(ShowStudio)));
            contextMenu1.Items.Add(new ToolStripMenuItem("Settings", null, new EventHandler(ShowSettings)));
            if (VSTEffect.Instance.useEffect)
            {
                contextMenu1.Items.Add(new ToolStripMenuItem("VST Editor", null, new EventHandler(ShowVSTEditor)));
            }
            contextMenu1.Items.Add(new ToolStripSeparator());
            contextMenu1.Items.Add(new ToolStripMenuItem("Quit", null, new EventHandler(Quit_Click), "Quit"));

            trayIcon.ContextMenuStrip = contextMenu1;

            trayIcon.Visible = true;
        }

        static void ShowStudio(object? sender, System.EventArgs e)
        {
            if (Shared.SharedWindowInstances.Instance.studioWindow != null) {
                Shared.SharedWindowInstances.Instance.studioWindow.Show();
            }
            if (Shared.SharedWindowInstances.Instance.extraWindow != null)
            {
                Shared.SharedWindowInstances.Instance.extraWindow.Show();
            }
        }
        static void ShowSettings(object? sender, System.EventArgs e) {
            if (Shared.SharedWindowInstances.Instance.settingsWindow != null)
            {
                Shared.SharedWindowInstances.Instance.settingsWindow.Show();
            }
            else
            {
                Shared.SharedWindowInstances.Instance.settingsWindow = new Views.SettingsWindow();
                Shared.SharedWindowInstances.Instance.settingsWindow.Show();

            }
        }
        static void ShowVSTEditor(object? sender, System.EventArgs e)
        {
            if (Shared.SharedWindowInstances.Instance.VSTeditor != null)
            {
                Shared.SharedWindowInstances.Instance.VSTeditor.Show();
            }
            else
            {
                Shared.SharedWindowInstances.Instance.VSTeditor = new Views.VST.VSTPluginEditor();
                Shared.SharedWindowInstances.Instance.VSTeditor.Show();

            }
        }
        static void Quit_Click(object? sender, System.EventArgs e)
        {
            if (System.Windows.MessageBox.Show("By exiting ForgeAir, anything that is running in will stop\nContinue?", "Warning", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
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
