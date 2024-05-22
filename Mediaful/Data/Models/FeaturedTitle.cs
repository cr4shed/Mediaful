using System.ComponentModel.DataAnnotations;

namespace Mediaful.Data.Models
{
    /// <summary>
    /// Featured title class which contains the data for featured images and text
    /// on all pages.
    /// </summary>
    public class FeaturedTitle
    {
        /// <summary>
        /// Id of the feature.
        /// </summary>
        [Key]
        public int FeatureId { get; set; }

        /// <summary>
        /// Description of the feature.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Path to the image in the solution for the feature.
        /// </summary>
        public string ImgPath { get; set; }

        /// <summary>
        /// Expiry date of the feature. Expired features remain in the database but are not displayed.
        /// </summary>
        public DateTime Expiry { get; set; }
    }
}
