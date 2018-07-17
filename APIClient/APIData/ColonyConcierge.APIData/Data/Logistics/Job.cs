using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data.Logistics
{
    public class Job
    {
        public int ID { get; set; }


        public string DisplayName { get; set; }

        /// <summary>
        /// Determines the moment in time that the job is to be performed.
        /// </summary>
        /// <remarks>
        /// <note type="note">This is mutually exclusive with the <see cref="Date"/> Property.
        /// Depending on the job type, this may or may not be a valid field.
        /// </note>
        /// </remarks>
        public TimeStamp Time { get; set; }

        /// <summary>
        /// End time
        /// </summary>
        /// <remarks>
        /// This is the estimated or specified "end time" of the job. 
        /// This property may or may not make sense for a specific job type.
        /// <note type="note">This is mutually exclusive with the <see cref="EndDate"/> Property.
        /// Depending on the job type, this may or may not be a valid field.
        /// </note>
        /// </remarks>        
        public TimeStamp EndTime { get; set; }

        //public SimpleDate Date { get; set; }

        //public SimpleDate EndDate { get; set; }

        /// <summary>
        /// READ-ONLY property defining the parent job ID, if any.
        /// </summary>
        public int ParentJobID { get; set; }


        /// <summary>
        /// READ-ONLY This is true if the job is in such a state that it is being performed by a worker.
        /// </summary>
        public bool IsInProgress { get; set; }

        /// <summary>
        /// READ-ONLY This is true if the job is in such a state that it had completed the workflow and no further processing will take place.
        /// </summary>
        public bool IsEnded { get; set; }

        /// <summary>
        /// READ-ONLY property defining the child jobs of this job
        /// </summary>
        public List<int> ChildJobs { get; set; }

        /// <summary>
        /// READ-ONLY True if the job is a pickup. Typically a job might be either a pickup or a drop off, but not both. This could change in 
        /// future scenarios though. Also, "parent jobs" are typically neither pickup nor drop off.
        /// </summary>
        public bool IsPickup { get; set; }

        /// <summary>
        /// READ-ONLY True if the job is a drop. Typically a job might be either a pickup or a drop off, but not both. This could change in 
        /// future scenarios though. Also, "parent jobs" are typically neither pickup nor drop off.
        /// </summary>
        public bool IsDropOff { get; set; }


        /// <summary>
        /// READ-ONLY  returns a list of IDs of sibling jobs (prerequisite) that must be completed before completing this job.
        /// </summary>
        public List<int> PrerequisiteSiblings { get; set; }

    }
}
