using ForgeAir.Core.Helpers;
using ForgeAir.Core.Services;
using ForgeAir.Core.Services.Importers;
using ForgeAir.Core.Services.Importers.Interfaces;
using ForgeAir.Core.Tracks;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;
using HandyControl.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Win32;
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
using MessageBox = HandyControl.Controls.MessageBox;
using Track = ForgeAir.Database.Models.Track;

namespace ForgeAir.Playout.Views
{
    /// <summary>
    /// Interaction logic for ImportTrackFileView.xaml
    /// </summary>
    public partial class ImportTrackFileView : HandyControl.Controls.GlowWindow
    {
        public ImportTrackFileView()
        {
            InitializeComponent();
        }
    }
}
