using Mediaful.Data.Models;
using Mediaful.Data.Services;
using Mediaful.Validation;
using MudBlazor;

namespace Mediaful.Pages.Admin
{
    /// <summary>
    /// Backend for the Announcements page.
    /// </summary>
    public partial class Announcements
    {
        /// <summary>
        /// MudForm reference.
        /// </summary>
        private MudForm form;

        /// <summary>
        /// AnnouncementValidator object to validate input fields.
        /// </summary>
        private AnnouncementValidator validator { get; set; } = new();

        /// <summary>
        /// Empty notification which the input fields bind to.
        /// </summary>
        private Notification announcement { get; set; } = new()
        {
            Comment = null
        };

        /// <summary>
        /// List of previously sent notifications which can be managed.
        /// </summary>
        private List<Notification> announcements;

        /// <summary>
        /// Initializes the backend of the page.
        /// </summary>
        /// <returns>Task.</returns>
        protected override async Task OnInitializedAsync()
        {
            await RefreshAnnouncements();
        }

        /// <summary>
        /// Method to refresh the previously sent announcements in descending order.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task RefreshAnnouncements()
        {
            announcements = await DatabaseService.GetRecords<Notification>(n => n.SenderId == 0) ?? new();
            announcements = announcements.OrderByDescending(n => n.NotificationId).ToList();
            StateHasChanged();
        }

        /// <summary>
        /// Method which handles the submission of a new announcement.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task Submit()
        {
            // Check if form input if valid.
            await form.Validate();
            if (!form.IsValid) return;

            // Set announcement information and add it to the database.
            announcement.SenderId = 0;
            announcement.Title = "[SYSTEM ANNOUNCEMENT]";
            await DatabaseService.Insert(announcement);

            // Use snackbar to display success message.
            Snackbar.Clear();
            Snackbar.Add($"Announcement sent successfully", Severity.Success);

            // Add announcement to the announcements list.
            announcements.Add(announcement);

            // Reset announcement input object and form.
            announcement = new()
            {
                Comment = null
            };
            await form.ResetAsync();
            form.ResetValidation();

            StateHasChanged();
        }

        /// <summary>
        /// Method to delete a previously sent announcement.
        /// </summary>
        /// <param name="id">Id of the announcement to delete.</param>
        /// <returns>Task.</returns>
        private async Task Delete(int id)
        {
            // Show a dialog box to confirm the user wants to delete the announcement.
            bool? result = await DialogService.ShowMessageBox(
                "Delete",
                "Are you sure you want to delete this announcement?",
                yesText: "Delete", cancelText: "Cancel");

            // If user confirms deletion.
            if (result != null)
            {
                // Delete notification.
                await DatabaseService.Delete<Notification>(id);

                var oldAnnouncement = announcements.Where(a => a.NotificationId == id).FirstOrDefault();
                announcements.Remove(oldAnnouncement);

                // Use snackbar to display success message.
                Snackbar.Clear();
                Snackbar.Add($"Successfully deleted", Severity.Success);
            }

            StateHasChanged();
        }
    }
}
