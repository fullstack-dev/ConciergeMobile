using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    [Flags]
    public enum ServiceDaysOfWeek
    {
        Invalid     = 128,
        Sunday      = 1,
        Monday      = 2,
        Tuesday     = 4,
        Wednesday   = 8,
        Thursday    = 16,
        Friday      = 32,
        Saturday    = 64
    }
}
