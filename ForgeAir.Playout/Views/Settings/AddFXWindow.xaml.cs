using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using ForgeAir.Core.AudioEngine;
using ForgeAir.Core.Shared;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using Microsoft.Win32;

namespace ForgeAir.Playout.Views
{
    /// <summary>
    /// Interaction logic for AddFXWindow.xaml
    /// </summary>
    public partial class AddFXWindow : HandyControl.Controls.Window
    {
        private string fxName;
        private string fxColor;
        private string fxFilename;
        private FX fx;
        private OpenFileDialog openFileDialog;
        private ForgeAirDbContext _context;
        public AddFXWindow()
        {
            openFileDialog = new OpenFileDialog();

            openFileDialog.CheckFileExists = true;
            openFileDialog.Multiselect = false;
            openFileDialog.CheckPathExists = true;

            fx = new FX();
            _context = new ForgeAirDbContext();
            InitializeComponent();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            fxName = titleBox.Text;
            fxFilename = fileDirBox.Text;
            fxColor = colorPicker.SelectedBrush.Color.ToString();

            fx = new FX() { Name = fxName, FilePath = fxFilename, Color = fxColor, DateAdded = DateTime.UtcNow, DateModified = DateTime.UtcNow, fxStatus = Database.Models.Enums.TrackStatus.Enabled};
            Task.Run(() => addFX(fx));
        }

        private async Task addFX(FX fx)
        {
            var tagReader = new TagReader();
            fx.Duration = tagReader.getDuration(new Track { FilePath = fx.FilePath });

            await _context.Fx.AddAsync(fx);
            await _context.SaveChangesAsync();
            DatabaseSharedData.Instance.RaiseDBModified();

        }
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dirSelect_Click(object sender, RoutedEventArgs e)
        {
            if (openFileDialog.ShowDialog() == false)
            {
                return;
            }
            else { fxFilename = openFileDialog.FileName; fileDirBox.Text = openFileDialog.FileName; }
        }
    }
}
