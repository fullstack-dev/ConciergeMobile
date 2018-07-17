using ColonyConcierge.Client.PlatformServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.Windows.PlatformServices
{
    class ConfigImpl : IConfig
    {
        public string this[string key]
        {
            get
            {
                return ConfigurationManager.AppSettings[key];
            }
        }

        public bool CheckValueExists(string name)
        {
            return ConfigurationManager.AppSettings.AllKeys.Contains(name);
            
        }

    }
}
