using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.PlatformServices
{
    public interface IConfig
    {

        bool CheckValueExists(string name);


        string this[string key]
        {
            get;
        }
    }
}
