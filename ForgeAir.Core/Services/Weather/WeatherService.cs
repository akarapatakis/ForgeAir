using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OpenSky.Core.Configuration;
using OpenSky.Core.Service;

namespace ForgeAir.Core.Services.Weather
{
   public class WeatherService : IWeatherService
   {

        public double[] CurrentWeather { get; set; }


        public WeatherService() {
            DotNetEnv.Env.TraversePath().Load();
        } 
        public async Task<double[]> GetWeather(string location) {
            var ossConfig = new OssConfiguration { ApiKey = System.Environment.GetEnvironmentVariable("WEATHERAPI_KEY") };
            var httpClient = new HttpClient();
            var ossService = new OpenSkyService(httpClient, ossConfig);
            var jsonResult = await ossService.GetForecastJsonAsync(location);
            if (jsonResult.IsSuccess)
            {
                var searchResults = jsonResult.Response;
                JObject data = JObject.Parse(searchResults);

                double tempC = data["current"]["temp_c"].Value<double>();
                double tempF = data["current"]["temp_f"].Value<double>();
                return new double[] {tempC, tempF };
            }
            else
            {
                Debug.WriteLine($"Error: {jsonResult.Error!.Message}");
                return new double[] { double.NaN, double.NaN };
            }
        }
   }
}
