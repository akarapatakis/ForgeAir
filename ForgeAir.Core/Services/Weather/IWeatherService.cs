using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenSky.Core.Configuration;
using OpenSky.Core.Service;


namespace ForgeAir.Core.Services.Weather
{
    public interface IWeatherService
    {
        double[] CurrentWeather { get; set; }
        Task<double[]> GetWeather(string location);
    }
}
