using ColonyConcierge.Client.PlatformServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.Windows.PlatformServices
{
    class TraceImpl : ITrace
    {
        public void Write(string message, string category)
        {
            System.Diagnostics.Trace.Write(message, category);
        }

        public void WriteLine(string message, string category)
        {
            System.Diagnostics.Trace.WriteLine(message, category);
        }
    }
}
