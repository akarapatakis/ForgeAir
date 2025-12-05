using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace ForgeAir.Playout.Helpers
{
    public static class ControlsHelper
    {
        public static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T)
                    return (T)current;

                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }
        public static ListViewItem GetNearestContainer(UIElement element)
        {
            // Finds the ListViewItem under the mouse
            ListViewItem container = element as ListViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as ListViewItem;
            }
            return container;
        }
    }
}
