using ForgeAir.Core.DTO;
using MahApps.Metro.IconPacks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.CustomCollections
{
    public class LinkedListQueue<T> : IEnumerable<T>
    {
        private LinkedList<T> list = new LinkedList<T>();

        public void EnqueueAtBottom(T item) => list.AddLast(item);
        public void EnqueueAtTop(T item) => list.AddFirst(item);

        public void Remove(T item) => list.Remove(item);
        public void EnqueueAt(T item, int index)
        {
            if (index < 0 || index > list.Count)
                EnqueueAtBottom(item);

            if (index == list.Count)
            {
                list.AddLast(item);
            }
            else
            {
                var current = list.First;
                for (int i = 0; i < index; i++)
                {
                    current = current.Next;
                }

                list.AddBefore(current, item);
            }
        }
        public T Dequeue()
        {
            if (list.Count == 0)
                throw new InvalidOperationException("Queue is empty");

            T value = list.First.Value;
            if (list.First.Value == null)
            {
                return default(T);
            }
            list.RemoveFirst();
            return value;
        }

        public T DequeueSpecificValue(T value)
        {
            if (list.Count == 0)
                return default(T);

            LinkedListNode<T> node = list.Find(value);

            if (node == null)
                return default(T);

            list.Remove(node);
            return node.Value;
        }
        public T Peek()
        {
            if (list.Count == 0)
                throw new InvalidOperationException("Queue is empty");

            return list.First.Value;
        }

        public T GetIndexOfValue(T value) => list.Find(value).Value;
        public bool IsEmpty() => list.Count == 0;

        public int Count() => list.Count;

        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }


    public class LinkedListQueueItem
    {
        public int? Place { get; set; }
        public TrackDTO Track { get; set; }

        public DateTime WillPlay { get; set; }
        public string WillPlayString { get => $"@ {WillPlay.ToString("HH:mm:ss")}"; }


        public string DisplayTitle => Track.Title;
        public string DisplayArtists
        {
            get
            {
                return Track.DisplayArtists;
            }
        }
        public string DisplayCategories
        {
            get
            {
                return Track.DisplayCategories ?? "None";
            }
        }
        public string Background => "#70" + Core.Helpers.TrackTypeColorGen.Generate(Track.TrackType).Substring(1); // add 0.7 opacity to have readable foreground because i thought that burning a person's eyes is a good idea :/
        public string? Foreground { get
            {
                if (Track.TrackType == Database.Models.Enums.TrackType.Rebroadcast || Track.IsDynamicJingleAsset)
                {
                    return "White";
                }
                return Track.Categories.FirstOrDefault()?.Color ?? "White";
            } }
        public string DisplayDuration => Track.Duration.ToString(@"mm\:ss");
        public PackIconRemixIconKind Icon => Core.Helpers.TrackTypeIconGen.Generate(Track.TrackType);


    }
}
