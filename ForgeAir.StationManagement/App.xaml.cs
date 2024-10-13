using ForgeAir.Database;
using ForgeAir.StationManagement.HostBuilders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ForgeAir.StationManagement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost host;

        public App()
        {
            host = CreateHostBuilder().Build();
        }

        public static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .AddServices();
        }

      
    }

}
