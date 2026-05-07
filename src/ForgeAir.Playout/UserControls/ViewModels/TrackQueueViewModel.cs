using Caliburn.Micro;
using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Events;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Playout.ViewModels.Helpers;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ForgeAir.Playout.UserControls.ViewModels
{
    public class TrackQueueViewModel : Screen, IDisposable
    {
        private readonly IEventAggregator _events;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<LinkedListQueueItem> frontendQueue { get; set; } = new ObservableCollection<LinkedListQueueItem>();


        private readonly IQueueService queueService;
        private readonly IAudioService _audioService;
        public IServiceProvider _provider { get; set; }
        public ICommand RemoveFromQueueCommand => new RelayCommand<LinkedListQueueItem>(RemoveFromQueue);
        public ICommand SkipToCommand => new RelayCommand<LinkedListQueueItem>(SkipTo);
        private QueueUpdatedEvent _queueUpdatedEvent;
        private TrackChangedEvent _trackChangedEvent;
        protected void OnPropertyChanged(string propName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        
        public TrackQueueViewModel(IServiceProvider provider, IAudioService audioService, QueueUpdatedEvent queueUpdatedEvent, TrackChangedEvent trackChangedEvent)
        {
            _provider = provider;
            _queueUpdatedEvent = queueUpdatedEvent;
            _trackChangedEvent = trackChangedEvent;
            _audioService = audioService;   
            frontendQueue = _provider.GetRequiredService<ObservableCollection<LinkedListQueueItem>>();
            _queueUpdatedEvent.QueueChanged += OnQueueChanged;
            queueService = _provider.GetRequiredService<IQueueService>();
            RefreshAndSyncQueues();
        }

        public void SkipTo(LinkedListQueueItem track)
        {
            int index = frontendQueue.IndexOf(track);

            if (index < 0) return;

            for (int i = 0; i <= index; i++)
            {
                if (i == index)
                {
                    _audioService.Play(true);
                    RefreshAndSyncQueues();
                    break;

                }
                queueService.Remove(queueService.Queue.ElementAt(0));
                RefreshAndSyncQueues();

            }


        }

        public DateTime CalculateWillPlayTime(LinkedListQueueItem track)
        {
            DateTime dt = DateTime.Now;
            if (track.Track.Duration == null)
            {
                return dt;
            }
            if (_trackChangedEvent.CurrentTrack == null)
            {
                return dt;
            }
            foreach (var item in frontendQueue)
            {

                
                if (track.Place == 1)
                {
                    if (_trackChangedEvent.CurrentTrack.Duration == null)
                    {
                        return dt;

                    }
                    dt = dt.Add(_trackChangedEvent.CurrentTrack.Duration); // include now playing
                    dt = dt.Add(item.Track.Duration); 
                }
                else
                {

                    dt = dt.Add(item.Track.Duration);

                }
            }

            return dt;
        }

        public void RemoveFromQueue(LinkedListQueueItem track)
        {
            queueService.Remove(track.Track);
            RefreshAndSyncQueues();
        }

        private LinkedListQueueItem MapToQueueItem(TrackDTO track, int index)
        {
            return new LinkedListQueueItem
            {
                Track = track,
                Place = index,
            };
        }
        private void RefreshAndSyncQueues()
        {
            frontendQueue.Clear();

            int index = 0;
            foreach (var track in queueService.Queue)
            {
                var item = MapToQueueItem(track, index);
                frontendQueue.Add(item);
                index++;
                item.WillPlay = CalculateWillPlayTime(item);

            }
        }


        public void MoveToQueue(LinkedListQueueItem item)
        {
            if (queueService.Contains(item.Track))
                queueService.Remove(item.Track);

            if (item.Place == null || item.Place < 0 || item.Place >= queueService.Count())
            {
                queueService.EnqueueBottom(item.Track);
            }
            else
            {
                queueService.Insert(item.Track, item.Place.Value);
            }

            RefreshAndSyncQueues();
        }

        private void OnQueueChanged()
        {
            Application.Current.Dispatcher.Invoke(() => {
                RefreshAndSyncQueues();
            });
        }

        public void Dispose()
        {
            _queueUpdatedEvent.QueueChanged -= OnQueueChanged;
        }
    }
}
