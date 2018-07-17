using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data.Logistics
{
    /// <summary>
    /// Data structure used to report on the completeness of jobs. 
    /// </summary>
    public class JobCompletionData
    {

        /// <summary>
        /// The job ID of the job that is being edited.
        /// </summary>
        public int JobID { get; set; }

        /// <summary>
        /// This should be set to "true" if the job was successful, otherwise set it to false/
        /// </summary>
        public bool SuccessfulCompletion { get; set; }


        [Obsolete("This was a typo misspelled name, use the SuccessfulCompletion property!")]
        public bool SucessfulCompletion
        {
            get
            {
                return SuccessfulCompletion;
            }
            set
            {
                SuccessfulCompletion = value;
            }
        }

        /// <summary>
        /// An optional note the will be placed on the job's history entry.
        /// </summary>
        public string ProblemNote { get; set; }
    }
}
