using Microsoft.AspNetCore.Components.Authorization;

namespace Mediaful.Data.Services
{
    /// <summary>
    /// Authentication service class which handles fetching information about
    /// an authenticated user using the application.
    /// </summary>
    public class AuthService : IAuthService
    {
        /// <summary>
        /// Authentication state provider object.
        /// Used to get information about the authenticated user.
        /// </summary>
        private readonly AuthenticationStateProvider authProvider;

        /// <summary>
        /// Constructor which uses dependency injection from Program.cs.
        /// </summary>
        /// <param name="authProvider">Authentication state provider.</param>
        public AuthService(AuthenticationStateProvider authProvider)
        {
            this.authProvider = authProvider;
        }

        /// <summary>
        /// Method to get the authenticated user's unique identifier.
        /// </summary>
        /// <returns>User's unique identifier.</returns>
        public async Task<int> GetUserId()
        {
            try
            {
                // Get the authenticated user.
                var user = (await authProvider.GetAuthenticationStateAsync()).User;

                // Return the user's unique identifier.
                return int.Parse(user.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value ?? "-1");
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        /// <summary>
        /// Method to get the authenticated user's username.
        /// </summary>
        /// <returns>User's username.</returns>
        public async Task<string> GetUserName()
        {
            // Get the authenticated user.
            var user = (await authProvider.GetAuthenticationStateAsync()).User;

            // Return username.
            return user.Identity.Name;
        }
    }
}
