using Mediaful.Shared.Enum;
using System.ComponentModel.DataAnnotations;


namespace Mediaful.Data.Models
{
    /// <summary>
    /// Class which stores data about a user's report on a notification.
    /// </summary>
    public class NotificationReport
    {
        /// <summary>
        /// Report Id.
        /// </summary>
        [Key]
        public int ReportId { get; set; }

        /// <summary>
        /// Id of the notification that was reported.
        /// </summary>
        public int NotificationId { get; set; }

        /// <summary>
        /// Lookup property used to access the notification that was reported.
        /// </summary>
        public Notification Notification { get; set; }

        /// <summary>
        /// Id of the user who submitted the report.
        /// </summary>
        public int ReporterId { get; set; }

        /// <summary>
        /// Status of the report. Uses ReportStatusEnum.
        /// </summary>
        public int Status { get; set; } = (int)ReportStatusEnum.Pending;
    }
}
