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
using System.Windows.Shapes;

namespace ForgeAir.Playout.Views
{
    /// <summary>
    /// Interaction logic for AddCategory.xaml
    /// </summary>
    public partial class AddCategory : HandyControl.Controls.Window
    {
        Core.Tracks.Importer importer;

        public AddCategory()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            Database.Models.Category category = new Database.Models.Category
            {
                Name = titleBox.Text,
                Description = descriptionBox.Text,
                Color = colorPicker.SelectedBrush.Color.ToString()
                
            };
            importer = new Core.Tracks.Importer();
            importer.AddCategory(category);
        }
    }
}
