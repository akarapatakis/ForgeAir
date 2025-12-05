using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Helpers.Interfaces
{
    public interface IConfigurationManager
    {
        string Get(string section, string key, string defaultValue = "");
        bool GetBool(string section, string key, bool defaultValue = false);
        int GetInt(string section, string key, int defaultValue = 0);
        void Set(string section, string key, string value);
        List<Dictionary<string, string>> GetAll(string section);
        void Save();
    }
}
