// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Mediaful.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Mediaful.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            //_emailStore = GetEmailStore();
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ProviderDisplayName { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        /// <summary>
        /// Not in use, removed from scaffolded identity files.
        /// </summary>
        /// <returns>404 error.</returns>
        public IActionResult OnGet() => RedirectToPage("~/404");

        /// <summary>
        /// Not in use, removed from scaffolded identity files.
        /// </summary>
        /// <returns>404 error.</returns>
        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            return Redirect("~/404");
        }

        /// <summary>
        /// Not in use, removed from scaffolded identity files.
        /// </summary>
        /// <returns>404 error.</returns>
        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            return Redirect("~/404");
        }

        /// <summary>
        /// Not in use, removed from scaffolded identity files.
        /// </summary>
        /// <returns>404 error.</returns>
        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            return Redirect("~/404");
        }

        /// <summary>
        /// Not in use, removed from scaffolded identity files.
        /// </summary>
        /// <returns>404 error.</returns>
        private ApplicationUser CreateUser()
        {
            return null;
        }

        /// <summary>
        /// Not in use, removed from scaffolded identity files.
        /// </summary>
        /// <returns>404 error.</returns>
        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            return null;
        }
    }
}
