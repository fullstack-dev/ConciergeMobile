using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data.Logistics
{
    public class WorkerManagementInfo
    {

        public User User { get; set; }

        public List<int> ActiveJobIDs { get; set; }

        public WorkerClockInInfo ClockInInfo { get; set; }

        public int CompletedJobsCount { get; set; }

        public TimeStamp LastJobCompletionTime { get; set; }

        public TimeStamp LastJobOfferTime { get; set; }

        public TimeStamp LastJobAssignmentTime { get; set; }

        public JobOfferResult LastJobOfferResult { get; set; }
        
    }

    public enum JobOfferResult
    {
        Invalid,
        Accepted,
        Rejected,
        TimedOut
    }
}
