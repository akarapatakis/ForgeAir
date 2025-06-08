using System;
using System.Collections.Generic;
using System.IO;

namespace ForgeAir.Core.Helpers
{
    public class ConfigurationManager
    {
        private Dictionary<string, Dictionary<string, string>> data = new();
        private string filePath;

        public ConfigurationManager(string path)
        {
            filePath = path;
            Load();
        }

        private void Load()
        {
            if (!File.Exists(filePath)) return;

            string section = "";
            foreach (var line in File.ReadLines(filePath))
            {
                string trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith(";")) continue;

                if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                {
                    section = trimmed[1..^1];
                    if (!data.ContainsKey(section)) data[section] = new();
                }
                else if (section != "")
                {
                    var parts = trimmed.Split('=', 2);
                    if (parts.Length == 2)
                        data[section][parts[0].Trim()] = parts[1].Trim();
                }
            }
        }

        public string Get(string section, string key, string defaultValue = "")
        {
            return data.ContainsKey(section) && data[section].ContainsKey(key)
                ? data[section][key]
                : defaultValue;
        }

        public int GetInt(string section, string key, int defaultValue = 0)
        {
            return int.TryParse(Get(section, key), out int result) ? result : defaultValue;
        }

        public bool GetBool(string section, string key, bool defaultValue = false)
        {
            return GetInt(section, key, defaultValue ? 1 : 0) == 1;
        }

        public void Set(string section, string key, string value)
        {
            if (!data.ContainsKey(section)) data[section] = new();
            data[section][key] = value;
        }

        public void Save()
        {
            using StreamWriter writer = new(filePath);
            foreach (var section in data)
            {
                writer.WriteLine($"[{section.Key}]");
                foreach (var kv in section.Value)
                    writer.WriteLine($"{kv.Key}={kv.Value}");
                writer.WriteLine();
            }
        }
    }

}