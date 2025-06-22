using ForgeAir.Core.Shared;
using ForgeAir.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for PlaybackQueue.xaml
    /// </summary>
    public partial class PlaybackQueue : System.Windows.Controls.UserControl
    {
        private ObservableCollection<DTO.QueueItem> _items = new ObservableCollection<DTO.QueueItem>();
        private ObservableCollection<Database.Models.Track> _queueItems = new ObservableCollection<Database.Models.Track>();

        public PlaybackQueue()
        {
            AudioPlayerShared.Instance.updateQueueUI += updateQueue;

            InitializeComponent();
            queueBox.ItemsSource = _items; // Bind the ListView to the ObservableCollection
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HandyControl.Controls.MessageBox.Show("Button clicked!");
        }

        private void skipButton_click(object sender, RoutedEventArgs e)
        {
            SkipToTrack();
        }

        private async void SkipToTrack()
        {
            int index = queueBox.SelectedIndex;

            if (index < 0 || index >= _queueItems.Count)
            {
                HandyControl.Controls.MessageBox.Show("No track selected or invalid selection.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                queueBox.SelectedIndex = -1;

                return;
            }
            for (int i = 0; i < index; i++)
            {
                this.Dispatcher.Invoke(() =>
                {

                    // Remove track from backend queue
                    AudioPlayerShared.Instance.trackQueue.DequeueSpecificValue(_queueItems[i]);

                    // Remove track from frontend queue
                    _items.RemoveAt(i);
                });
                
            }

            // fill queue
            await updateQueueAsync();
            Task.Run(() => AudioPlayerShared.Instance.audioPlayer.Play());

        }

        
        private void updateQueue(object sender, EventArgs e) {

            this.Dispatcher.Invoke(() =>
            {
                _items.Clear();

                _queueItems.Clear();
                foreach (var item in AudioPlayerShared.Instance.trackQueue)
                {
                    QueueItem queueItem = new DTO.QueueItem { Track = item, Place = queueBox.Items.Count + 1 };
                    _items.Add(queueItem);
                    _queueItems.Add(item);
                }
            });
            return;
        }
        private Task updateQueueAsync()
        {
            this.Dispatcher.Invoke(() =>
            {
                _items.Clear();

                _queueItems.Clear();
                foreach (var item in AudioPlayerShared.Instance.trackQueue)
                {
                    QueueItem queueItem = new DTO.QueueItem {Track = item, Place = queueBox.Items.Count + 1 };
                    _items.Add(queueItem);
                    _queueItems.Add(item);
                }
            });
            return Task.CompletedTask;
        }
        private void removeFromQueue(object sender, RoutedEventArgs e)
        {
            int index = queueBox.SelectedIndex;
            if (index < 0 || index >= _queueItems.Count)
            {
                HandyControl.Controls.MessageBox.Show("No track selected or invalid selection.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Remove track from backend queue
            AudioPlayerShared.Instance.trackQueue.DequeueSpecificValue(_queueItems[index]);

            // Remove track from frontend queue
            _items.RemoveAt(index);

            // fill queue
            AudioPlayerShared.Instance.RaiseOnQueueChanged();
        }
    }

}
