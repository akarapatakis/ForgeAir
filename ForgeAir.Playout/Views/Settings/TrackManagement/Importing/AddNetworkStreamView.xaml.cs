using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
using ForgeAir.Core.Services;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Core.Tracks;
using ForgeAir.Database.Models;
using Microsoft.EntityFrameworkCore;
using ForgeAir.Database;
using System.Net.Http;
using System.Net.Sockets;
using ForgeAir.Core.Services.Database.Interfaces;
using ForgeAir.Core.Services.Importers;
using ForgeAir.Core.Services.Importers.Interfaces;

namespace ForgeAir.Playout.Views
{
    /// <summary>
    /// Interaction logic for AddNetworkStreamView.xaml
    /// </summary>
    public partial class AddNetworkStreamView : HandyControl.Controls.Window
    {
        public AddNetworkStreamView()
        {
            InitializeComponent();
        }

    }
    
}
