using Mediaful.Data.Models;
using Mediaful.Shared.Enum;
using Mediaful.Validation;
using MudBlazor;

namespace Mediaful.Pages.User
{
    /// <summary>
    /// Backend for the Log Title page.
    /// </summary>
    public partial class LogTitle
    {
        /// <summary>
        /// Flag that indicates if the user is sharing logged watch data with others or not.
        /// </summary>
        private bool share = true;

        /// <summary>
        /// Flag which indicates the page is loading.
        /// </summary>
        private bool isLoading = false;

        /// <summary>
        /// Flag which indicates if the user is banned from sharing.
        /// </summary>
        private bool isUserBanned = false;

        /// <summary>
        /// Comment to share with others if sharing is true.
        /// </summary>
        private string comment = string.Empty;

        /// <summary>
        /// WatchData input object.
        /// </summary>
        private WatchData watchData = new();

        /// <summary>
        /// Validator to validate the input.
        /// </summary>
        private WatchDataValidator validator = new();

        /// <summary>
        /// List of the user's existing logged titles.
        /// Used for updating existing watch data.
        /// </summary>
        private List<WatchData> userData;

        /// <summary>
        /// List of autocomplete options for the title input field.
        /// </summary>
        private List<string> options = new();

        /// <summary>
        /// MudChip reference for editing the status of a log.
        /// </summary>
        private MudChip statusChip;

        /// <summary>
        /// MudChip reference for editing the rating of a log.
        /// </summary>
        private MudChip ratingChip;

        /// <summary>
        /// MudChip reference for editing the favourite flag of a log.
        /// </summary>
        private MudChip favChip;

        /// <summary>
        /// MudForm reference.
        /// </summary>
        private MudForm form;

        /// <summary>
        /// Autocomplete title input field reference.
        /// </summary>
        private MudAutocomplete<string> autocomplete;

        /// <summary>
        /// Initializes the backend of the page.
        /// </summary>
        /// <returns>Task.</returns>
        protected override async Task OnInitializedAsync()
        {
            int userId = await AuthService.GetUserId();

            // If user is not authenticated, data cannot be loaded. Front-end handles redirect.
            if (userId == -1) return;

            // Get all existing watch data for the user and add the titles to options.
            userData = await DatabaseService.GetRecords<WatchData>(wd => wd.UserId == userId);
            if (userData != null)
            {
                userData.ForEach(x => options.Add(x.Title));
            }

            // Check user's banned status.
            var user = await DatabaseService.GetRecord<ApplicationUser>(au => au.Id == userId);
            isUserBanned = user.IsBanned;
            if (isUserBanned)
            {
                share = false;
            }
        }

        /// <summary>
        /// Method to handle autocomplete search.
        /// </summary>
        /// <param name="value">Search string.</param>
        /// <returns>IEnumerable of titles that contain the search string.</returns>
        private async Task<IEnumerable<string>> SearchAsync(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return options;
            }

            return options.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Method to handle autocomplete selection change.
        /// </summary>
        /// <param name="title">Title to change to.</param>
        /// <returns>Task.</returns>
        private async Task OnSelectionChange(string title)
        {
            isLoading = true;
            
            // If watch data for title exists already, fetch it from the database.
            if (!string.IsNullOrEmpty(title) && userData.Exists(ud => ud.Title.ToLower() == title.ToLower()))
            {
                int userId = await AuthService.GetUserId();

                watchData = await DatabaseService.GetRecord<WatchData>(wd => wd.UserId == userId && wd.Title == title);
            }

            // Else reset input object and set title.
            else
            {
                statusChip = null;
                ratingChip = null;
                favChip = null;
                watchData.Title = title;
                watchData.IsTvShow = false;
                watchData.Status = (int)StatusEnum.Undefined;
                watchData.Rating = (int)RatingEnum.Undefined;
                watchData.IsFavorite = false;
                watchData.TvEpisode = null;
                watchData.TvSeason = null;
                watchData.FirstWatchDate = null;
                watchData.LastWatched = null;
                watchData.TimesWatched = 0;
                watchData.WatchDataId = 0;
            }
            isLoading = false;
            StateHasChanged();
        }

        /// <summary>
        /// Method to validate the length of the comment input field.
        /// </summary>
        /// <param name="input">String user input.</param>
        /// <returns>Error message if validation fails.</returns>
        private IEnumerable<string> ValidateCommentLength(string input)
        {
            if (!string.IsNullOrEmpty(input) && input?.Length > 512)
                yield return "Max 512 characters";
        }

        /// <summary>
        /// Method to handle submitting the watch data.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task Submit()
        {
            // Check if form input if valid.
            await form.Validate();
            if (!form.IsValid) return;

            int userId = await AuthService.GetUserId();

            // Get chip values.
            watchData.Status = statusChip == null ? 0 : (int)statusChip.Value;
            watchData.Rating = ratingChip == null ? 0 : (int)ratingChip.Value;
            watchData.IsFavorite = favChip == null || ((int)favChip?.Value) != 0 ? false : true;

            // Log watch data to the database.
            await DatabaseService.LogWatchData(userId, watchData);

            // Handle sharing if applicable.
            if (share && !isUserBanned)
            {
                // Create and insert new notification into the database.
                // Uses a snapshot of the WatchData values.
                WatchData updatedData = await DatabaseService.GetRecord<WatchData>(watchData.WatchDataId);
                Notification notification = new Notification()
                {
                    SenderId = userId,
                    Title = updatedData.Title,
                    Status = updatedData.Status,
                    TimesWatched = updatedData.TimesWatched,
                    Rating = updatedData.Rating,
                    IsTvShow = updatedData.IsTvShow,
                    TvSeason = updatedData.TvSeason,
                    TvEpisode = updatedData.TvEpisode,
                    IsFavorite = updatedData.IsFavorite,
                    Comment = comment ?? string.Empty
                };

                await DatabaseService.Insert(notification);
            }

            // Use snackbar to display success message.
            Snackbar.Clear();
            Snackbar.Add($"Successfully added {watchData.Title} to your collection", Severity.Success);

            // Reset input fields.
            watchData = new();
            comment = string.Empty;
            await autocomplete.ResetAsync();
            favChip = null;
            statusChip = null;
            ratingChip = null;
        }
    }
}
