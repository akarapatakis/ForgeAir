using ForgeAir.Core.AudioEngine;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Helpers;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Playout.Helpers;
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

namespace ForgeAir.Playout.UserControls.Views
{
    /// <summary>
    /// Interaction logic for TrackSelectorControl.xaml
    /// </summary>
    public partial class TrackSelectorView : UserControl
    {
        public TrackSelectorViewModel ViewModel { get; }
        private Point _startPoint;
        public TrackSelectorView()
        {
            InitializeComponent();


        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

    
        private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private void ListView_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(null);
            var diff = _startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var listView = sender as ListView;
                var data = listView?.SelectedItem;
                var pos = e.GetPosition(listView);
                var element = listView.InputHitTest(pos) as DependencyObject;
                var listViewItem = ControlsHelper.FindAncestor<ListViewItem>(element);

                if (listViewItem == null || listViewItem.IsSelected == false)
                    return;
                if (data != null)
                {
                    DragDrop.DoDragDrop(listView, data, DragDropEffects.Move);
                }
            }
        }

        private void TrackItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item && item.DataContext is TrackDTO track)
            {
                ((TrackSelectorViewModel)DataContext).DoubleClickAdd(track);
            }

        }
    }

}
