using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using ForgeAir.UI.Core.Models;

namespace ForgeAir.UI.Core.UserControls
{
    /// <summary>
    /// Interaction logic for CheckComboBox.xaml
    /// </summary>
    public partial class CheckComboBox : UserControl
    {
        public ObservableCollection<SelectableItem> Items
        {
            get { return (ObservableCollection<SelectableItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<SelectableItem>),
                typeof(CheckComboBox), new PropertyMetadata(new ObservableCollection<SelectableItem>()));

        public string SelectedSummary
        {
            get
            {
                var selected = Items?.Where(i => i.IsChecked).Select(i => i.Name).ToList();
                return (selected != null && selected.Any()) ? string.Join(", ", selected) : "None selected";
            }
        }

        public CheckComboBox()
        {
            InitializeComponent();

            // Refresh summary when item selection changes
            Loaded += (s, e) =>
            {
                foreach (var item in Items)
                    item.PropertyChanged += (s2, e2) =>
                    {
                        if (e2.PropertyName == nameof(SelectableItem.IsChecked))
                            OnPropertyChanged(nameof(SelectedSummary));
                    };
            };

            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
