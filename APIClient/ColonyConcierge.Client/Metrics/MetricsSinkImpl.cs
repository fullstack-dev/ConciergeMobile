using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.Metrics
{
    class MetricsSinkImpl : IMetricsSink
    {

        class MethodCallStats
        {
            public int CallCount;

            public List<long> CallTimes = new List<long>();

        }

        #region Private Data
        Dictionary<string, MethodCallStats> _stats;
        #endregion


        public MetricsSinkImpl()
        {
            _stats = new Dictionary<string, MethodCallStats>();
        }

        public void RecordEntry(string endpoint, long runtime_ms)
        {
            MethodCallStats stat;

            lock (this)
            {
                if (_stats.ContainsKey(endpoint))
                {
                    stat = _stats[endpoint];
                }
                else
                {
                    stat = new MethodCallStats();
                    _stats.Add(endpoint, stat);
                }

                stat.CallCount++;

                stat.CallTimes.Add(runtime_ms);
            }

        }

        public void ResetCounts()
        {
            lock (this)
            {
                _stats = new Dictionary<string, MethodCallStats>();
            }
            
        }

        public string Dump()
        {
            lock(this)
            {
                StringBuilder result = new StringBuilder();

                var coallated = from kvp in _stats
                                select new
                                {
                                    Method = kvp.Key,
                                    Count = kvp.Value.CallCount,
                                    FastestTime = kvp.Value.CallTimes.Min(),
                                    SlowestTime = kvp.Value.CallTimes.Max(),
                                    AverageTime = kvp.Value.CallTimes.Average(),
                                    TotalTime = kvp.Value.CallTimes.Sum(),
                                };

                var sorted = from s in coallated
                             orderby s.TotalTime descending
                             select s;

                string formatString = "{0,50},{1, -10},{2, -10},{3, -10},{4, -10},{5, -10}";
                result.AppendLine(string.Format(formatString,
                    "Method", "Count", "Min Time", "Max Time", "Ave Time", "Tot Time"
                    ));

                foreach( var s in sorted)
                {
                    result.AppendLine(string.Format(formatString, s.Method, s.Count, s.FastestTime, s.SlowestTime, s.AverageTime, s.TotalTime));
                }

                return result.ToString();
            }
        }
    }
}
