using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Avalonia.VisualTree;

namespace ForgeAir.Playout.App.Helpers
{
    public static class ControlsHelper
    {
        public static T? FindAncestor<T>(Avalonia.Visual? current)
            where T : Avalonia.Visual
        {
            while (current != null)
            {
                if (current is T match)
                    return match;

                current = current.GetVisualParent();
            }

            return null;
        }


        public static Avalonia.Controls.ListBoxItem? GetNearestContainer(Avalonia.Visual element)
        {
            var current = element;

            while (current != null)
            {
                if (current is Avalonia.Controls.ListBoxItem container)
                    return container;

                current = current.GetVisualParent();
            }

            return null;
        }
    }
}
