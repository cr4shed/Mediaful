using Mediaful.Data.Models;
using Mediaful.Data.Services;
using Mediaful.Shared.Enum;
using MudBlazor;

namespace Mediaful.Pages.User
{
    /// <summary>
    /// Backend for the Index/Dashboard page.
    /// </summary>
    public partial class Index
    {
        /// <summary>
        /// Integer constant which defines how many notification records to incrementally query from the database.
        /// </summary>
        private const int takeCount = 15;

        /// <summary>
        /// Flag to check if the page is loading.
        /// </summary>
        private bool isLoading = true;

        /// <summary>
        /// Flag which represents if the user can continue incrementally loading notifications.
        /// </summary>
        private bool canQueryNotifications = true;

        /// <summary>
        /// List of notifications to display to the user.
        /// </summary>
        private List<Notification> notifications = new();

        /// <summary>
        /// Dictionary of key, value pairs for userid, username lookups.
        /// </summary>
        private Dictionary<int, string> usernames = new();

        /// <summary>
        /// List of reported notifications for the authenticated user.
        /// Only populated if user is authenticated.
        /// </summary>
        private List<NotificationReport> reports = new();

        /// <summary>
        /// Initializes the backend of the page.
        /// </summary>
        /// <returns>Task.</returns>
        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            int userId = await AuthService.GetUserId();

            // Get notifications to display to user.
            await GetNotifications();

            // Get notification reports if user is authenticated.
            reports = await DatabaseService.GetRecords<NotificationReport>(nr => nr.ReporterId == userId);

            isLoading = false;
            StateHasChanged();
        }

        /// <summary>
        /// Method to incrementally get notifications for an authenticated user's dashboard.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task GetNotifications()
        {
            int userId = await AuthService.GetUserId();

            // Get {takeCount} notifications on an offset of {notifications.Count()}.
            List<Notification> newNotifications = await DatabaseService.GetNotifications(userId, takeCount, notifications.Count());

            // If newNotifications is null or contains < {takeCount}, there are no more records that can be retrieved.
            if (newNotifications == null) return;
            else if (newNotifications.Count < takeCount)
            {
                // Hide the "load more" button.
                canQueryNotifications = false;
            }

            foreach (Notification notification in newNotifications)
            {
                // If the notification's sender is already in the dictionary, continue.
                if (usernames.ContainsKey(notification.SenderId)) continue;

                // Add userid, username to the dictionary.
                ApplicationUser user = await DatabaseService.GetRecord<ApplicationUser>(au => au.Id == notification.SenderId);
                string username = user?.UserName ?? string.Empty;

                usernames.Add(notification.SenderId, username);
            }

            // Append newly retrieved notifications to the notifications list for display.
            notifications.AddRange(newNotifications);

            StateHasChanged();
        }

        /// <summary>
        /// Helper method to generate the system generated text of a notification based on the WatchData information provided.
        /// </summary>
        /// <param name="notif">Notification to get the message for.</param>
        /// <returns>Generated string message.</returns>
        private string GetDefaultMessage(Notification notif)
        {
            // Message for a user adding a title to their watchlist.
            if (notif.Status == (int)StatusEnum.Watchlist)
            {
                return $"{GetUsername(notif.SenderId)} has added <strong><span class=\"text-primary word-wrap\">{notif.Title}</span></strong> to their watchlist!";
            }
            
            // Message for a user actively watching a title.
            else if (notif.Status == (int)StatusEnum.Watching)
            {
                return $"{GetUsername(notif.SenderId)} has started watching <strong><span class=\"text-primary word-wrap\">{notif.Title}</span></strong>";
            }

            // Message for a user that has finished watching a title.
            else if (notif.Status == (int)StatusEnum.Watched)
            {
                string msg = $"{GetUsername(notif.SenderId)} has finished watching ";
                string season  = string.Empty;

                if (notif.IsTvShow && notif.TvSeason != null)
                {
                    string episode = string.Empty;
                    if (notif.TvEpisode != null)
                    {
                        episode = $", Episode {notif.TvEpisode}";
                    }

                    msg+= $"Season {notif.TvSeason}{episode} of ";
                }
                msg += $"<strong><span class=\"text-primary word-wrap\">{notif.Title}</span></strong>!";
                return msg;
            }

            // Generic message where no tags were added to the WatchData.
            else
            {
                return $"{GetUsername(notif.SenderId)} has added <strong><span class=\"text-primary word-wrap\">{notif.Title}</span></strong> to their collection!";
            }
        }

        /// <summary>
        /// Method for a user to report a notification in their feed.
        /// Only available for authenticated users.
        /// </summary>
        /// <param name="notificationId">Id of the notification to report.</param>
        /// <returns>Task.</returns>
        private async Task Report(int notificationId)
        {
            // Show a dialog box to confirm the user wants to report the notification.
            bool? result = await DialogService.ShowMessageBox(
                "Report",
                "Are you sure you want to report this comment?",
                yesText: "Report", cancelText: "Cancel");

            // If user confirms report.
            if (result != null)
            {
                // Create new NotificationReport object.
                int userId = await AuthService.GetUserId();
                NotificationReport report = new()
                {
                    NotificationId = notificationId,
                    ReporterId = userId
                };

                // Insert the report into the database.
                await DatabaseService.Insert(report);
                reports.Add(report); //= await DatabaseService.GetRecords<NotificationReport>(nr => nr.ReporterId == userId);

                // Use snackbar to display success message.
                Snackbar.Clear();
                Snackbar.Add($"Successfully reported", Severity.Success);
            }

            StateHasChanged();
        }

        /// <summary>
        /// Helper method to check if a notification has already been reported by the user.
        /// </summary>
        /// <param name="notificationId">Notification id to check if a report exists.</param>
        /// <returns>True if user has reported the notification, otherwise false.</returns>
        private bool IsReported(int notificationId)
        {
            return reports.Exists(r => r.NotificationId == notificationId);
        }

        /// <summary>
        /// Helper method to get usernames from the dictionary.
        /// </summary>
        /// <param name="userId">User id key used to find the corresponding username.</param>
        /// <returns>Username associated with the user id.</returns>
        private string GetUsername(int userId)
        {
            return usernames.GetValueOrDefault(userId);
        }
    }
}
