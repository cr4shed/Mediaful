using System.Security.Claims;

namespace Mediaful.Shared.Layout
{
    /// <summary>
    /// Backend for the top layout.
    /// </summary>
    public partial class TopLayout
    {
        /// <summary>
        /// User authentication state.
        /// </summary>
        private ClaimsPrincipal user;
        protected override async Task OnInitializedAsync()
        {

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            user = authState.User;
        }

        /// <summary>
        /// Method used to refresh the component.
        /// </summary>
        public void RefreshComponent()
        {
            StateHasChanged();
        }
    }
}
