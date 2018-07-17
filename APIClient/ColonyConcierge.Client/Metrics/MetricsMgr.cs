using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.Metrics
{
    public static class MetricsMgr
    {

        #region Private Data
        static MetricsSinkImpl _instance;
        #endregion

        public static IMetricsSink Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(MetricsMgr))
                    {
                        if (_instance == null)
                        {
                            _instance = new MetricsSinkImpl();
                        }
                    }
                }

                return _instance;
            }
        }

    }
}
