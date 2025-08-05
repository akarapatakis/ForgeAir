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
using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;
using ForgeAir.Playout.UserControls.ViewModels;

namespace ForgeAir.Playout.UserControls.Views
{
    /// <summary>
    /// Interaction logic for TrackQueueView.xaml
    /// </summary>
    public partial class TrackQueueView : UserControl
    {
        private Point _dragStartPoint;
        private object _draggedItem;

        public TrackQueueView()
        {
            InitializeComponent();
        }
        private int GetCurrentIndex(ListView listView, Point clickedPoint)
        {
            for (int i = 0; i < listView.Items.Count; i++)
            {
                var item = listView.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                if (item != null)
                {
                    Rect bounds = VisualTreeHelper.GetDescendantBounds(item);
                    Point topLeft = item.TranslatePoint(new Point(0, 0), listView);

                    if (clickedPoint.Y >= topLeft.Y && clickedPoint.Y <= topLeft.Y + bounds.Height)
                    {
                        return i;
                    }
                }
            }

            return listView.Items.Count;
        }
        private void Queue_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void Queue_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }
        private void Queue_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = _dragStartPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                ListView listView = sender as ListView;
                ListViewItem listViewItem = Helpers.ControlsHelper.FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

                if (listViewItem != null)
                {
                    _draggedItem = listView.ItemContainerGenerator.ItemFromContainer(listViewItem);
                    DragDrop.DoDragDrop(listViewItem, _draggedItem, DragDropEffects.Move);
                }
            }
        }

        private void Queue_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TrackDTO)))
            {

                var droppedData = _draggedItem;
                var _listView = sender as ListView;
                var target = Helpers.ControlsHelper.GetNearestContainer(e.OriginalSource as UIElement);
                if (target == null)
                    return;

                var targetItem = _listView.ItemContainerGenerator.ItemFromContainer(target);

                var items = (ObservableCollection<LinkedListQueueItem>)_listView.ItemsSource;
                int oldIndex = items.IndexOf((LinkedListQueueItem)droppedData);
                int newIndex = items.IndexOf((LinkedListQueueItem)targetItem);

                if (oldIndex == -1 || newIndex == -1)
                {
                    return;
                }
                if (oldIndex != newIndex)
                {
                    items.Move(oldIndex, newIndex);
                }

                _draggedItem = null;
                return;
            }

            var track = (TrackDTO)e.Data.GetData(typeof(TrackDTO));
            if (track == null)
                return;

            var listView = sender as ListView;
            if (listView == null)
                return;

            Point dropPosition = e.GetPosition(listView);
            int index = GetCurrentIndex(listView, dropPosition);

            var queueItem = new LinkedListQueueItem { Track = track, Place = index };

            if (DataContext is TrackQueueViewModel vm)
            {
                vm.MoveToQueue(queueItem);
            }
        }
    }
}
