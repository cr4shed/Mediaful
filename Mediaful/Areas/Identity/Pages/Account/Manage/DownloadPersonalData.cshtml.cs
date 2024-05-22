// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Mediaful.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mediaful.Areas.Identity.Pages.Account.Manage
{
    public class DownloadPersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DownloadPersonalDataModel> _logger;

        public DownloadPersonalDataModel(
            UserManager<ApplicationUser> userManager,
            ILogger<DownloadPersonalDataModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Not in use, removed from scaffolded identity files.
        /// </summary>
        /// <returns>404 error.</returns>
        public IActionResult OnGet()
        {
            return Redirect("~/404");
        }

        /// <summary>
        /// Not in use, removed from scaffolded identity files.
        /// </summary>
        /// <returns>404 error.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            return Redirect("~/404");
        }
    }
}
