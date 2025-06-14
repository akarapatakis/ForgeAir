using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ForgeAir.Core;
using ForgeAir.Core.AudioEngine;
using ForgeAir.Core.Services.Models;
using ForgeAir.Core.Shared;
using ForgeAir.Playout.Views.VST;
using Microsoft.Win32;


namespace ForgeAir.Playout.Views
{
    /// <summary>
    /// Interaction logic for StudioWindow.xaml
    /// </summary>
    public partial class StudioWindow : HandyControl.Controls.Window    
    {
        DeviceManager deviceManager;

        Microsoft.Win32.OpenFileDialog openFileDialog;


        public StudioWindow()
        {

            InitializeComponent();


            AudioPlayerShared.Instance.audioPlayer = new AudioPlayer();
            var floatingWindow = new UserControls.ClockControl();

            // Set Initial Position
            Canvas.SetLeft(floatingWindow, 100);
            Canvas.SetTop(floatingWindow, 100);

            // Add to Canvas
            // FloatingCanvas.Children.Add(floatingWindow);


        }

        private void button_Click(object sender, RoutedEventArgs e)
        {


            Task.Run(() => AudioPlayerShared.Instance.audioPlayer.Play());


        }

        private void button2_Copy1_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => AudioPlayerShared.Instance.audioPlayer.Play());

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            AudioPlayerShared.Instance.audioPlayer.PauseResumeHandler();
        }

        private void button2_Copy_Click(object sender, RoutedEventArgs e)
        {
            AudioPlayerShared.Instance.audioPlayer.Stop();
        }

        private void button3_Copy1_Click(object sender, RoutedEventArgs e)
        {
            AudioPlayerShared.Instance.currentTrack = new Database.Models.Track();

            AudioPlayerShared.Instance.currentTrack.FilePath = openFileDialog.FileNames[1];
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {

            openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Multiselect = false;
            openFileDialog.CheckPathExists = true;
            openFileDialog.ShowDialog();
            Track instant = new Track();
            instant.FilePath = openFileDialog.FileName;


        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            Views.ImportTrackFileView view = new Views.ImportTrackFileView();
            view.ShowDialog();
        }

        private void button_Copy21_Click(object sender, RoutedEventArgs e)
        {
            Views.TracksList view = new Views.TracksList();
            view.ShowDialog();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Views.AddCategory view = new Views.AddCategory();
            view.Show();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                AudioPlayerShared.Instance.audioPlayer.Play();
            });


        }

        private void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            Views.ImportFolder view = new Views.ImportFolder();
            view.Show();
        }

        private void settingsbtn_Click(object sender, RoutedEventArgs e)
        {
            Views.SettingsWindow view = new Views.SettingsWindow();
            view.Show();
        }

        private void vstSettingsBtn_Click(object sender, RoutedEventArgs e)
        {


        }

        private void TrackSelectorControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (AudioPlayerShared.Instance.autoStart) {
                Task.Run(() =>
                {
                    AudioPlayerShared.Instance.audioPlayer.Play();
                });

            }
            return;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           // int SliderValue = (int)trackslider.Value;

            // Overloaded constructor takes the arguments days, hours, minutes, seconds, milliseconds.
            // Create a TimeSpan with miliseconds equal to the slider value.
          //  TimeSpan ts = TimeSpan.FromSeconds(SliderValue);

            Task.Run(() =>
                {
               //     AudioPlayerShared.Instance.audioPlayer.SeekTo(ts);
                });
        }
    }
}