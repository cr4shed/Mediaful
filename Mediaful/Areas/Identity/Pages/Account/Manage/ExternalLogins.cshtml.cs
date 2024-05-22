// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Mediaful.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mediaful.Areas.Identity.Pages.Account.Manage
{
    public class ExternalLoginsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserStore<ApplicationUser> _userStore;

        public ExternalLoginsModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserStore<ApplicationUser> userStore)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userStore = userStore;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> OtherLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool ShowRemoveButton { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        /// Not in use, removed from scaffolded identity files.
        /// </summary>
        /// <returns>404 error.</returns>
        public async Task<IActionResult> OnGetAsync()
        {
            return Redirect("~/404");
        }

        /// <summary>
        /// Not in use, removed from scaffolded identity files.
        /// </summary>
        /// <returns>404 error.</returns>
        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            return Redirect("~/404");
        }

        /// <summary>
        /// Not in use, removed from scaffolded identity files.
        /// </summary>
        /// <returns>404 error.</returns>
        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            return Redirect("~/404");
        }

        /// <summary>
        /// Not in use, removed from scaffolded identity files.
        /// </summary>
        /// <returns>404 error.</returns>
        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            return Redirect("~/404");
        }
    }
}
