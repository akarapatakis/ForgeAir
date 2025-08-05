using Caliburn.Micro;
using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Models;
using ForgeAir.Playout.ViewModels.Helpers;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ForgeAir.Playout.UserControls.ViewModels
{
    public class TrackQueueViewModel : Screen
    {
        private readonly IEventAggregator _events;

        public ObservableCollection<LinkedListQueueItem> Queue {get; set;}
        public ObservableCollection<TrackDTO> _items { get; set; }
        public IServiceProvider _provider { get; set; }
        public ICommand RemoveFromQueueCommand => new RelayCommand<LinkedListQueueItem>(RemoveFromQueue);
        public ICommand SkipToCommand => new RelayCommand<LinkedListQueueItem>(SkipTo);

        public TrackQueueViewModel(IServiceProvider provider)
        {
            _provider = provider;
            _items = _provider.GetRequiredService<ObservableCollection<TrackDTO>>();
            Queue = _provider.GetRequiredService<ObservableCollection<LinkedListQueueItem>> ();
        }

        public void SkipTo(LinkedListQueueItem track)
        {
            int index = Queue.IndexOf(track);
            {
                for (int i = 0; i <= index; i++)
                {
                    Queue.RemoveAt(0);
                }
            }
        }

        public void RemoveFromQueue(LinkedListQueueItem track)
        {
            if (Queue.Contains(track))
                Queue.Remove(track);
        }
        public void MoveToQueue(LinkedListQueueItem item)
        {
            if (_items.Contains(item.Track))
                _items.Remove(item.Track);

            if (item.Place < 0 || item.Place > Queue.Count)
                Queue.Add(item);
            else
                Queue.Insert(item.Place, item);
        }

    }
}
