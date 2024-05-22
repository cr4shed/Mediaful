using Mediaful.Data.Models;
using Mediaful.Data.Services;
using Mediaful.Shared.Enum;
using MudBlazor;

namespace Mediaful.Pages.Admin
{
    /// <summary>
    /// Backend for the Notification Reports page.
    /// </summary>
    public partial class NotificationReports
    {
        /// <summary>
        /// List of notification reports.
        /// </summary>
        private List<NotificationReport> reports = new();

        /// <summary>
        /// Initializes the backend of the page.
        /// </summary>
        /// <returns>Task.</returns>
        protected async override Task OnInitializedAsync()
        {
            await RefreshReports();
        }

        /// <summary>
        /// Method to update the notification reports and order them by the report status.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task RefreshReports()
        {
            reports = await DatabaseService.GetRecords<NotificationReport>(includes: x => x.Notification);
            reports = reports.OrderBy(r => r.Status).ToList();
        }

        /// <summary>
        /// Method to mark a report as safe and reviewed.
        /// </summary>
        /// <param name="id">Id of the report to mark as safe.</param>
        /// <returns>Task.</returns>
        private async Task SetSafe(int id)
        {
            // Show a dialog box to confirm the user wants to mark the report as safe.
            bool? result = await DialogService.ShowMessageBox(
                "Dismiss",
                "Are you sure you want to dismiss this report?",
                yesText: "Dismiss", cancelText: "Cancel");

            // If user confirms safe.
            if (result != null)
            {
                // Mark report status as safe. This means the report has been reviewed and dismissed.
                NotificationReport report = await DatabaseService.GetRecord<NotificationReport>(nr => nr.ReportId == id);
                report.Status = (int)ReportStatusEnum.Safe;
                await DatabaseService.Update(report);
                
                reports.Find(r => r.ReportId == id).Status = (int)ReportStatusEnum.Safe;

                // Use snackbar to display success message.
                Snackbar.Clear();
                Snackbar.Add($"Successfully deleted", Severity.Success);
            }

            StateHasChanged();
        }

        /// <summary>
        /// Method to mark a report as unsafe and remove it from viewing.
        /// </summary>
        /// <param name="id">Id of the report to mark as unsafe.</param>
        /// <returns>Task.</returns>
        private async Task SetUnsafe(int id)
        {
            // Show a dialog box to confirm the user wants to mark the report as unsafe and delete the comment.
            bool? result = await DialogService.ShowMessageBox(
                "Delete",
                "Are you sure you want to remove this comment?",
                yesText: "Delete", cancelText: "Cancel");

            // If user confirms unsafe.
            if (result != null)
            {
                // Mark report status as unsafe. This means the comment has been removed.
                // But the user and notification remain unactioned.
                NotificationReport report = await DatabaseService.GetRecord<NotificationReport>(nr => nr.ReportId == id);
                report.Status = (int)ReportStatusEnum.Unsafe;
                await DatabaseService.Update(report);

                // Remove comment from notification.
                Notification notif = await DatabaseService.GetRecord<Notification>(n => n.NotificationId == report.NotificationId);
                notif.Comment = string.Empty;
                await DatabaseService.Update(notif);

                reports.Find(r => r.ReportId == id).Status = (int)ReportStatusEnum.Unsafe;
                reports.Find(r => r.ReportId == id).Notification = notif;

                // Use snackbar to display success message.
                Snackbar.Clear();
                Snackbar.Add($"Successfully deleted", Severity.Success);
            }

            StateHasChanged();
        }

        /// <summary>
        /// Method to mark a report as unsafe, remove it from viewing, and disable the sending user.
        /// </summary>
        /// <param name="id">Id of the report to mark as unsafe.</param>
        /// <returns>Task.</returns>
        private async Task DeleteAndDisable(int id)
        {
            // Show a dialog box to confirm the user wants to mark the report as unsafe, delete the comment, and disable the sending user.
            bool? result = await DialogService.ShowMessageBox(
                "Delete",
                "Are you sure you want to remove this notification and disable the user?",
                yesText: "Delete", cancelText: "Cancel");

            // If user confirms unsafe.
            if (result != null)
            {
                // Delete report.
                NotificationReport report = await DatabaseService.GetRecord<NotificationReport>(nr => nr.ReportId == id);
                await DatabaseService.Delete(report);

                // Delete notification.
                Notification notif = await DatabaseService.GetRecord<Notification>(n => n.NotificationId == report.NotificationId);
                notif.Comment = string.Empty;
                await DatabaseService.Delete(notif);

                // Mark user as banned from sharing.
                var user = await DatabaseService.GetRecord<ApplicationUser>(au => au.Id == notif.SenderId);
                user.IsBanned = true;
                await DatabaseService.Update(user);

                var oldReport = reports.Where(r => r.ReportId == id).FirstOrDefault();
                reports.Remove(oldReport);

                // Use snackbar to display success message.
                Snackbar.Clear();
                Snackbar.Add($"Successfully deleted", Severity.Success);
            }

            StateHasChanged();
        }
    }
}

