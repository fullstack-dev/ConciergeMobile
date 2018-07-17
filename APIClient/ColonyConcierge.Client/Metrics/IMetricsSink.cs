using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.Metrics
{
    public interface IMetricsSink
    {
        void RecordEntry(string endpoint, long runtime_ms);


        void ResetCounts();

        string Dump();
    }
}
