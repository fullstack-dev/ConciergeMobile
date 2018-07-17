using ColonyConcierge.Client.PlatformServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.iOS.PlatformServices
{
    class FactoryImpl : IFactory
    {
        public T Create<T>(params object[] creationParamters)
        {
            throw new NotImplementedException();
        }
    }
}
