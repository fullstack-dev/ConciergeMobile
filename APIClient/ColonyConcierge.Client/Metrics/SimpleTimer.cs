using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.Metrics
{
    /// <summary>
    /// simple class to track time between creation and disposing. Intended for internal use only.
    /// </summary>


    class SimpleTimer : IDisposable //TODO proper isdispose pattern
    {


        #region private data
        bool _enable;
        string _endpoint;
        IMetricsSink _sink;
        Stopwatch _stopWatch;
        #endregion

        public SimpleTimer(IMetricsSink sink, bool enable, string endpoint)
        {
            _enable = enable;
            if (enable)
            {
                _sink = sink;
                _endpoint = endpoint;
                _stopWatch = new Stopwatch();
                _stopWatch.Start();

            }
        }


        ~SimpleTimer()
        {
            Dispose(false);
        }

        public static SimpleTimer Measure(IMetricsSink sink, bool enable, string enpoint)
        {
            return new SimpleTimer(sink, enable, enpoint);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        void Dispose(bool isDisposing)
        {
            if (_enable)
            {
                _stopWatch.Stop();
            }
            GC.SuppressFinalize(this);

            //only collect metrics when we explicitly disposed, otherwise the metrics are meaningless. 
            if (isDisposing)
            {
                if (_enable)
                {
                    _sink.RecordEntry(_endpoint, _stopWatch.ElapsedMilliseconds);
                }
            }
        }
    }
}
