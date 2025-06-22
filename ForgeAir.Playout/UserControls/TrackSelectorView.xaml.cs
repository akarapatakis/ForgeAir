using ForgeAir.Core.AudioEngine;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Helpers;
using ForgeAir.Core.Shared;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Playout.UserControls.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ForgeAir.Playout.UserControls
{
    /// <summary>
    /// Interaction logic for TrackSelectorControl.xaml
    /// </summary>
    public partial class TrackSelectorView : UserControl
    {
        public TrackSelectorViewModel ViewModel { get; }

        public TrackSelectorView()
        {
            InitializeComponent();

            // Resolve dependencies manually or inject via a factory/service locator
            var sp = ((App)Application.Current).ServiceProvider;

            ViewModel = sp.GetRequiredService<TrackSelectorViewModel>();

            DataContext = ViewModel;
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listView.SelectedItem is TrackDTO track)
            {
                ViewModel.AddTrackToQueue(track);
            }
        }
    }

}
