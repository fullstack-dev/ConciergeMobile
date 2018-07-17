using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.PlatformServices
{
    public interface ITrace
    {
        void Write(string message, string category);
        void WriteLine(string message, string category);

    }
}
