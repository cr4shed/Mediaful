using Mediaful.Data.Models;
using Mediaful.Data.Services;
using Mediaful.Shared.Enum;

namespace Mediaful.Shared.Layout
{
    /// <summary>
    /// Backend for the sidebar layout.
    /// </summary>
    public partial class SidebarLayout
    {
        /// <summary>
        /// Titles to display in the top watched chart.
        /// </summary>
        private string[] titles = new string[0];

        /// <summary>
        /// Watch counts for each title in the top watched chart.
        /// </summary>
        private double[] counts = new double[0];

        /// <summary>
        /// Integer which displays a notification for admins if there are pending notification reports.
        /// </summary>
        private int reports = 0;

        /// <summary>
        /// Initializes the backend of the page.
        /// </summary>
        /// <returns>Task.</returns>
        protected override async Task OnInitializedAsync()
        {
            // When LayoutState notifies changes, call this component's StateHasChanged() method.
            LayoutState.OnChange += StateHasChanged;

            // Get watch statistics.
            Dictionary<string, int> dict = await DatabaseService.GetWatchStats();

            // Get number of pending notification reports. Hidden for all but admin users.
            var reportList = await DatabaseService.GetRecords<NotificationReport>(x => x.Status == (int)ReportStatusEnum.Pending, includes: x => x.Notification);
            reports = reportList?.Count() ?? 0;

            // Use dictionary to add to the titles and counts arrays for display.
            if (dict != null && dict.Count > 0)
            {
                int count = 0;
                titles = new string[dict.Count];
                counts = new double[dict.Count];

                foreach(var kv in dict)
                {
                    titles[count] = kv.Key;
                    counts[count] = kv.Value;
                    count++;
                }
            }
        }
    }
}
