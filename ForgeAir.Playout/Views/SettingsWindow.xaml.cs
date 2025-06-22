using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ForgeAir.Playout.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : HandyControl.Controls.GlowWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public void FeatureNotImplementedYet()
        {
            HandyControl.Controls.MessageBox.Show("This feature is currently not available at this version.", "Feature Not Implemented", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FeatureNotImplementedYet();
        }
        private void addDirBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Views.ImportFolder view = new Views.ImportFolder();
            view.Show();

        }
        private void addTrackBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Views.ImportTrackFileView view = new Views.ImportTrackFileView();
            view.ShowDialog();
        }

        private void addRebrodBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Views.AddRebroadcastSource view = new Views.AddRebroadcastSource();
            view.ShowDialog();
        }
        private void addFxBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Views.AddFXWindow view = new Views.AddFXWindow();
            view.ShowDialog();
        }

        private void addCatBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Views.AddCategory view = new Views.AddCategory();
            view.Show();
        }

        private void tracksManagerBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Views.TracksList view = new Views.TracksList();
            view.ShowDialog();
        }

        private void Border_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            Views.Settings.AudioConfigurationWindow view = new Settings.AudioConfigurationWindow();
            view.ShowDialog();
        }

        private void schedulerBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Views.Settings.MLSchedulerWindow view = new Settings.MLSchedulerWindow();
            view.ShowDialog();
        }
    }
}
