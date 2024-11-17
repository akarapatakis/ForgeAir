using ForgeAirPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.RDS
{
    public class DeviceManager
    {
        private readonly string _pluginDirectory;
        public List<IPluginEntry> encoders;

        public DeviceManager(string pluginDirectory)
        {
            _pluginDirectory = pluginDirectory;
            encoders = new List<IPluginEntry>();
        }

        /// <summary>
        /// Loads all dlls implementing IRDSDevice from the plugin directory.
        /// </summary>
        public IEnumerable<IPluginEntry> FindEncoders()
        {
            if (!Directory.Exists(_pluginDirectory))
                throw new DirectoryNotFoundException($"Plugin directory '{_pluginDirectory}' does not exist.");

            foreach (var file in Directory.GetFiles(_pluginDirectory, "*.dll"))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);

                    var pluginInstances = assembly.GetTypes()
                        .Where(type => typeof(IPluginEntry).IsAssignableFrom(type) &&
                                       typeof(IRDSDevice).IsAssignableFrom(type) &&
                                       !type.IsAbstract &&
                                       type.IsClass)
                        .Select(type => Activator.CreateInstance(type) as IPluginEntry)
                        .Where(plugin => plugin != null);

                    encoders.AddRange(pluginInstances);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading '{file}': {ex.Message}");
                }
            }

            return encoders;
        }

        /// <summary>
        /// Displays detailed information about the encoder.
        /// </summary>
        public string GetPluginDetails(IPluginEntry encoder)
        {
            if (encoder == null)
                throw new ArgumentNullException(nameof(encoder));

            return $"""
                Name: {encoder.name}
                Author: {encoder.author}
                Version: {encoder.version}
                Homepage: {encoder.homepage}
                Description: {encoder.description}
                """;
        }

        public void ShowConfigPage(IPluginEntry encoder)
        {
           encoder.ShowConfigurationPage();
        }
        public void ShowAboutPage(IPluginEntry encoder)
        {
            encoder.ShowAboutPage();
        }
        public void LoadEncoder(IRDSDevice encoder) {
            encoder = Shared.RDSParams.Instance.rdsEncoder; // hooks loaded encoder dll to Shared
        }
    }
}
