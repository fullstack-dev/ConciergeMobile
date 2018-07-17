using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data.Logistics.NotificationData
{
    public enum NotificationKind
    {
        /// <summary>
        /// For weekly schedules. The ID's of new allocated schedules will be provided
        /// </summary>
        NewWeeklyScheduleAvailable,
        /// <summary>
        /// For individual schedules. The ID's of new allocated schedules will be provided
        /// </summary>
        NewSchedulesAvailable,
        /// <summary>
        /// For a single job offer. The ID list will contain a single ID for the offered job.
        /// </summary>
        NewJobOffer,
        /// <summary>
        /// Sent when a job offer expired. The ID list will contain a single ID for the offered job.
        /// </summary>
        JobOfferExpired,
        /// <summary>
        /// For when an assignment is made. The ID list will contain a single ID for the assigned job.
        /// </summary>
        NewJobAssignment,
        /// <summary>
        /// This is sent when the user Claims an open job, and another user had also claimed the job
        /// In theory, if we ever support jobs being offered to more that one worker at a time, this 
        /// message could also be used to let the "loosing" worker know that someone else claimed the job first.
        /// The ID list will contain a single ID for the assigned job.
        /// </summary>
        ClaimRejected,
        /// <summary>
        /// When the user has been forcibly clocked out.
        /// </summary>
        ForcedClockout,
        /// <summary>
        /// Sent when it is time to start a job. The ID list will contain a single ID for the related job
        /// </summary>
        TimeToStartJob,
        /// <summary>
        /// Sent when an order is placed
        /// </summary>
        OrderPlaced,
        /// <summary>
        /// Sent when a payment is successful
        /// </summary>
        PaymentSucessful,
        /// <summary>
        /// Sent when a payment fails
        /// </summary>
        PaymentFailed,
        /// <summary>
        /// Sent when an order is canceled 
        /// </summary>
        OrderCanceled,
        /// <summary>
        /// Sent when an order is being prepared by fulfillment service (eg, a restaurant is preparing order
        /// </summary>
        OrderIsBeingPrepared,
        /// <summary>
        /// Sent when an order has begun the delivery process
        /// </summary>
        DeliveryStarted,
        /// <summary>
        /// Sent when an order has been delivered
        /// </summary>
        DeliveryComplete,
        /// <summary>
        /// (NOT USED YET) Sent when there is a location update for the order.
        /// </summary>
        DeliveryLocationUpdate,

        /// <summary>
        /// This notification is sent when a job that was previously assigned to the user is no longer assigned to them
        /// </summary>
        JobUnassigned,

        /// <summary>
        /// This notification is sent when a job is canceled.
        /// </summary>
        JobCanceled,

        /// <summary>
        /// This is a custom message that can be sent be a manager 
        /// </summary>
        ManagementToWorkerMessage,

        /// <summary>
        /// This is sent when it is time for the asystant to become active.
        /// </summary>
        TimeToGoActive,

        /// <summary>
        /// This is sent to 'nag' asystants about job offers that they have not yet accepted.
        /// </summary>
        JobOfferReminder,



    }
}
