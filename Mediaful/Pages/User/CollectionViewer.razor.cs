using Mediaful.Data.Models;
using Mediaful.Data.Services;
using Mediaful.Shared.Enum;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Mediaful.Pages.User
{
    /// <summary>
    /// Backend for the Collection Viewer page.
    /// </summary>
    public partial class CollectionViewer
    {
        /// <summary>
        /// Optional url parameter which indicates if the user is viewing their
        /// own profile, or another user's profile.
        /// </summary>
        [Parameter]
        public string UserId { get; set; }

        /// <summary>
        /// UserId parsed as an integer for easier use.
        /// </summary>
        private int IntUserId { get; set; }

        /// <summary>
        /// List of logged watch data in the user's collection.
        /// </summary>
        private List<WatchData> WatchData { get; set; } = new();

        /// <summary>
        /// Flag to signify if the user is viewing their own collection
        /// or another user's collection.
        /// </summary>
        private bool viewingOwnCollection { get; set; } = false;

        /// <summary>
        /// Default header for user viewing their own collection.
        /// </summary>
        private string header = "My Watch Collection";

        /// <summary>
        /// Default search string. Bound to the watch data table.
        /// </summary>
        private string searchString = string.Empty;

        /// <summary>
        /// WatchData reference object used to backup old values when a user edits a log.
        /// </summary>
        private WatchData editBackup;

        /// <summary>
        /// MudChip reference for editing the status of a log.
        /// </summary>
        private MudChip statusChip { get; set; }

        /// <summary>
        /// MudChip reference for editing the rating of a log.
        /// </summary>
        private MudChip ratingChip { get; set; }

        /// <summary>
        /// MudChip reference for editing the favourite flag of a log.
        /// </summary>
        private MudChip favChip { get; set; }

        /// <summary>
        /// Override to refresh the page data if the UserId url parameter is changed.
        /// </summary>
        /// <returns>Task.</returns>
        protected override async Task OnParametersSetAsync()
        {
            // Try to parse UserId parameter.
            int.TryParse(UserId, out int parsedUserId);

            // If UserId is valide, fetch the watch data for the collection.
            if (parsedUserId > 0 && IntUserId != parsedUserId || UserId == null)
            {
                await GetWatchData();
            }
        }

        /// <summary>
        /// Initializes the backend of the page.
        /// </summary>
        /// <returns>Task.</returns>
        protected async override Task OnInitializedAsync()
        {
            await GetWatchData();
        }

        /// <summary>
        /// Method to handle which collection of watch data is displayed to the user.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task GetWatchData()
        {
            // Get authenticated user's ID.
            int userId = await AuthService.GetUserId();

            // If unauthenticated user is attempting to access /log/ with no user parameter, redirect them to registration.
            if (string.IsNullOrEmpty(UserId) && userId == -1) NavManager.NavigateTo("/Identity/Account/Register");

            // Else if user is authenticated and did not supply a parameter, assume they are viewing their own collection.
            else if (string.IsNullOrEmpty(UserId))
            {
                IntUserId = userId;
                viewingOwnCollection = true;
                header = "My Watch Collection";
            }

            // Else assume another user's collection is being viewed.
            else
            {
                // Parse UserId and set the header to reflect who's collection is being viewed.
                int.TryParse(UserId, out int parsedUserId);
                if (parsedUserId > 0)
                {
                    ApplicationUser user = await DatabaseService.GetRecord<ApplicationUser>(au => au.Id == parsedUserId);
                    if (user != null)
                    {
                        string username = user.UserName;
                        IntUserId = parsedUserId;
                        header = $"{username}'s Watch Collection";
                    }
                }
                else
                {
                    NavManager.NavigateTo("/404");
                }
            }

            // Update the watch data from the set user's collection.
            await UpdateWatchData();
        }

        /// <summary>
        /// Method to update the watch data list for the set user's collection.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task UpdateWatchData()
        {
            WatchData = await DatabaseService.GetRecords<WatchData>(wd => wd.UserId == IntUserId);
            StateHasChanged();
        }

        /// <summary>
        /// Method used to delete a logged watch data.
        /// Only displayed for authenticated users viewing their own collection.
        /// </summary>
        /// <param name="id">Id of the watch data to delete.</param>
        /// <returns>Task.</returns>
        private async Task Delete(int id)
        {
            // Show a dialog box to confirm the user wants to delete the watch data.
            bool? result = await DialogService.ShowMessageBox(
                "Delete",
                "Are you sure you want to delete this log?",
                yesText: "Delete", cancelText: "Cancel");

            // If user confirms deletion.
            if (result != null)
            {
                // Delete watch data from database.
                await DatabaseService.Delete<WatchData>(id);

                // Delete from front-end list.
                WatchData wd = WatchData.Where(wd => wd.WatchDataId == id).FirstOrDefault();
                WatchData.Remove(wd);

                // Use snackbar to display success message.
                Snackbar.Clear();
                Snackbar.Add($"Successfully deleted", Severity.Success);
            }

            StateHasChanged();
        }

        /// <summary>
        /// Method called by MudTable to backup an item being edited by the user.
        /// Only authenticated users on viewing their own collection can use this feature.
        /// </summary>
        /// <param name="data">Generic object containing the backed up WatchData.</param>
        private void BackupItem(object data)
        {
            // Parse generic object and back it up.
            editBackup = new();
            editBackup.Title = ((WatchData)data).Title;
            editBackup.Status = ((WatchData)data).Status;
            editBackup.Rating = ((WatchData)data).Rating;
            editBackup.IsFavorite = ((WatchData)data).IsFavorite;
        }

        /// <summary>
        /// Method called by MudTable to reset edited changes.
        /// Only authenticated users on viewing their own collection can use this feature.
        /// </summary>
        /// <param name="data">Generic object containing the backed up WatchData.</param>
        private void ResetEdit(object data)
        {
            // Reset backup object.
            ((WatchData)data).Title = editBackup.Title;
            ((WatchData)data).Status = editBackup.Status;
            ((WatchData)data).Rating = editBackup.Rating;
            ((WatchData)data).IsFavorite = editBackup.IsFavorite;
        }

        /// <summary>
        /// Method called by MudTable to commit edited changes to the database.
        /// Only authenticated users on viewing their own collection can use this feature.
        /// </summary>
        /// <param name="data">Generic object containing the backed up WatchData.</param>
        private async void CommitEdit(object data)
        {
            // Parse generic object to get changes and update the record in the database.
            ((WatchData)data).Status = statusChip == null ? 0 : (int)statusChip.Value;
            ((WatchData)data).Rating = ratingChip == null ? 0 : (int)ratingChip.Value;
            ((WatchData)data).IsFavorite = favChip == null || ((int)favChip?.Value) != 0 ? false : true;
            await DatabaseService.Update((WatchData)data);
        }

        /// <summary>
        /// Method used to search the table using searchString.
        /// </summary>
        /// <param name="data">WatchData record to compare against the search string.</param>
        /// <returns>True if WatchData meets the search criteria, otherwise false.</returns>
        private bool Search(WatchData data)
        {
            // Show all results.
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            
            // Check contains substring.
            else if (data.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            // Check for television and related substrings. Returns true for records that are IsTvShow.
            else if (data.IsTvShow && (searchString.Contains("tv", StringComparison.OrdinalIgnoreCase) 
                        || searchString.Contains("television", StringComparison.OrdinalIgnoreCase)
                        || searchString.Contains("show", StringComparison.OrdinalIgnoreCase)))
                return true;

            // Check for movie substring. Returns true for records that are !IsTvShow (is movie).
            else if (!data.IsTvShow && searchString.Contains("movie", StringComparison.OrdinalIgnoreCase))
                return true;

            // Check for StatusEnum values.
            else if (((StatusEnum)data.Status).ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            // Check for RatingEnum values.
            else if (((RatingEnum)data.Rating).ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            // Check favourite.
            else if (data.IsFavorite && searchString.Contains("fav", StringComparison.OrdinalIgnoreCase))
                return true;

            // Check times watched.
            else if (data.TimesWatched.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            // Search criteria not met.
            return false;
        }
    }
}
