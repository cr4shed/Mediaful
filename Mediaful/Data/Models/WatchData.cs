using Mediaful.Shared.Enum;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Mediaful.Data.Models
{
    /// <summary>
    /// Class which stores watch data about a title a user has added to their collection.
    /// </summary>
    public partial class WatchData
    {
        /// <summary>
        /// Id of the watch data.
        /// </summary>
        [Key]
        public int WatchDataId { get; set; }

        /// <summary>
        /// Id of the user who is adding the watch data to their collection.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Title of the movie or television show.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Watch status of the title. Uses StatusEnum.
        /// </summary>
        public int Status { get; set; } = (int)StatusEnum.Undefined;

        /// <summary>
        /// Number of times a user has watched the title.
        /// </summary>
        [DisplayName("Times Watched")]
        public int TimesWatched { get; set; } = 0;

        /// <summary>
        /// Date the user first watched the title.
        /// </summary>
        [DisplayName("First Watch")]
        public DateTimeOffset? FirstWatchDate { get; set; }

        /// <summary>
        /// Date the user last watched the title.
        /// </summary>
        [DisplayName("Last Watch")]
        public DateTimeOffset? LastWatched { get; set; }

        /// <summary>
        /// User's rating of the title. Uses RatingEnum.
        /// </summary>
        public int Rating { get; set; } = (int)RatingEnum.Undefined;

        /// <summary>
        /// Flag to determine if the title is a movie or television show.
        /// </summary>
        public bool IsTvShow { get; set; } = false;

        /// <summary>
        /// Season the user has watched if IsTvShow is true.
        /// </summary>
        public int? TvSeason { get; set; }

        /// <summary>
        /// Episode the user has watched if IsTvShow is true.
        /// </summary>
        public int? TvEpisode { get; set; }

        /// <summary>
        /// Flag to determine if the title has been favourited by the user.
        /// </summary>
        [DisplayName("Favorited")]
        public bool IsFavorite { get; set; } = false;
    }
}