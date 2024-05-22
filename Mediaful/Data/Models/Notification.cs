using Mediaful.Shared.Enum;
using System.ComponentModel;

namespace Mediaful.Data.Models
{
    /// <summary>
    /// Notificaiton class which stores a snapshot of relevant WatchData
    /// information and a comment to share with other users on their dashboard.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Notification's Id.
        /// </summary>
        public int NotificationId { get; set; }

        /// <summary>
        /// Id of the user who created the notification.
        /// </summary>
        public int SenderId { get; set; }

        /// <summary>
        /// Snapshot of the title from the user's WatchData.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Snapshot of the status of the title from the user's WatchData. Uses StatusEnum.
        /// </summary>
        public int Status { get; set; } = (int)StatusEnum.Undefined;

        /// <summary>
        /// Snapshot of the number of times the user has watched the title from the user's WatchData.
        /// </summary>
        [DisplayName("Times Watched")]
        public int TimesWatched { get; set; } = 0;

        /// <summary>
        /// Snapshot of the title's rating from the user's WatchData. Uses RatingEnum.
        /// </summary>
        public int Rating { get; set; } = (int)RatingEnum.Undefined;

        /// <summary>
        /// Snapshot of if the WatchData is for a movie or television show.
        /// </summary>
        public bool IsTvShow { get; set; } = false;

        /// <summary>
        /// Snapshot of the TV season in the WatchData (where applicable, see IsTvShow).
        /// </summary>
        public int? TvSeason { get; set; }

        /// <summary>
        /// Snapshot of the TV episode in the WatchData (where applicable, see IsTvShow).
        /// </summary>
        public int? TvEpisode { get; set; }

        /// <summary>
        /// Snapshot of if the title is favourited in the WatchData.
        /// </summary>
        [DisplayName("Favorited")]
        public bool IsFavorite { get; set; } = false;

        /// <summary>
        /// User's comment on the title, shared with other users who can see the notification.
        /// </summary>
        public string Comment { get; set; } = string.Empty;
    }
}
