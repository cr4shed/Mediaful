using Mediaful.Data.Models;
using Mediaful.Data.Services;

namespace Mediaful.Pages.User
{
    /// <summary>
    /// Backend for the Search Users page.
    /// </summary>
    public partial class Users
    {
        /// <summary>
        /// Integer constant which defines how many user records to incrementally query from the database.
        /// </summary>
        private const int takeNumber = 20;

        /// <summary>
        /// Flag which represents if the user can continue incrementally loading users.
        /// </summary>
        private bool canQueryUsers = false;

        /// <summary>
        /// Search string to use to find users with matching usernames.
        /// </summary>
        private string searchString = string.Empty;

        /// <summary>
        /// Current authenticated user's id.
        /// </summary>
        private int userId = -1;

        /// <summary>
        /// List of users to display.
        /// </summary>
        private List<ApplicationUser> users = new();

        /// <summary>
        /// Initializes the backend of the page.
        /// </summary>
        /// <returns>Task.</returns>
        protected override async Task OnInitializedAsync()
        {
            userId = await AuthService.GetUserId();

            await SearchUsers(string.Empty);
        }

        /// <summary>
        /// Method to search for users that having usernames containing the search string.
        /// </summary>
        /// <param name="searchString">Search string to check useranmes for.</param>
        /// <returns>Task.</returns>
        private async Task SearchUsers(string searchString)
        {
            this.searchString = searchString;

            // Update users list.
            users = await DatabaseService.GetUsers(searchString, takeNumber, 0, userId);

            // Show "load more" button if database returned the maxmium number of records.
            canQueryUsers = users.Count < takeNumber ? false : true;

            StateHasChanged();
        }

        /// <summary>
        /// Method to incrementally load more user records matching the existing search string.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task LoadMoreUsers()
        {
            // Temporary list of new users that match the search string.
            List<ApplicationUser> newUsers = await DatabaseService.GetUsers(searchString, takeNumber, users.Count, userId);

            // If newUsers list is null or contains < {takeNumber}, retrieved all possible matching records.
            if (newUsers == null) return;
            else if (newUsers.Count < takeNumber)
            {
                // Hide "load more" button.
                canQueryUsers = false;
            }

            // Add new users to existing user list for display.
            users.AddRange(newUsers);

            StateHasChanged();
        }
    }
}
