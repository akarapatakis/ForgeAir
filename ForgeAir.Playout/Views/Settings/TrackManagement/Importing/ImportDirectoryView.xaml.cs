using ForgeAir.Core.Helpers;
using ForgeAir.Core.Services;
using ForgeAir.Core.Services.Importers;
using ForgeAir.Core.Tracks;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ForgeAir.Playout.Views
{
    /// <summary>
    /// Interaction logic for ImportDirectoryView.xaml
    /// </summary>
    public partial class ImportDirectoryView : HandyControl.Controls.GlowWindow
    {
        public ImportDirectoryView()
        {
            InitializeComponent();
        }
    }
}
