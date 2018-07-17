using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.PlatformServices
{
    public interface IFactory
    {

        T Create<T>(params object[] creationParamters);

    }
}
