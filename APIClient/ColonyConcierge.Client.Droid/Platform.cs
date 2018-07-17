using ColonyConcierge.Client.PlatformServices;
using ColonyConcierge.Client.Droid.PlatformServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.Droid
{
    public class Platform
    {

        public static void Init()
        {
            ProviderImpl.StaticInit();
            ProviderImpl.G.Init();
        }
    }
}
