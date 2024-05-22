using Microsoft.AspNetCore.Identity;

namespace Mediaful.Data.Models
{
    /// <summary>
    /// Application User class used for Identity.
    /// Inheirts ASP.NET default IdentityUser class with integer user ids.
    /// </summary>
    public class ApplicationUser : IdentityUser<int>
    {
        /// <summary>
        /// Bool flag which represents if a user has been banned from sharing with other users.
        /// </summary>
        public bool IsBanned { get; set; } = false;
    }
}
