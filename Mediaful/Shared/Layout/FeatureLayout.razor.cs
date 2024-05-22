using Mediaful.Data.Models;

namespace Mediaful.Shared.Layout
{
    /// <summary>
    /// Backend for the featured title layout.
    /// </summary>
    public partial class FeatureLayout
    {
        /// <summary>
        /// Random number generator.
        /// </summary>
        private Random rand = new Random();

        /// <summary>
        /// List of all featured titles.
        /// </summary>
        private List<FeaturedTitle> features = new();

        /// <summary>
        /// The currently displayed featured title.
        /// </summary>
        private FeaturedTitle currentFeature;

        /// <summary>
        /// Initializes the backend of the page.
        /// </summary>
        /// <returns>Task.</returns>
        protected override async Task OnInitializedAsync()
        {
            // Get all featured titles.
            features = await DatabaseService.GetRecords<FeaturedTitle>(ft => ft.Expiry > DateTime.Now);

            if (features.Count() > 0)
            {
                // Randomly choose a featured title to display.
                int index = rand.Next(0, features.Count);
                currentFeature = features.ElementAt(index);
            }
        }
    }
}
