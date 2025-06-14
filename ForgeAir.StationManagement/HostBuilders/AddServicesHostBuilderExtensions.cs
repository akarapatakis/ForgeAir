using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.StationManagement.HostBuilders
{
    public static class AddServicesHostBuilderExtensions
    {
        /// <summary>
        /// Inject services into the host
        /// </summary>
        /// <returns></returns>
        public static IHostBuilder AddServices(this IHostBuilder host)
        {
            host.ConfigureServices(services =>
            {

            });

            return host;
        }
    }
}
