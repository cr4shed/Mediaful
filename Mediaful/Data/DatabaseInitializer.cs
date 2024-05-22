using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Mediaful.Data
{
    /// <summary>
    /// Database initializer class which handles creating and seeding the database.
    /// </summary>
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Method to seed the database.
        /// </summary>
        /// <param name="serviceProvider">Service provider.</param>
        /// <returns>Task.</returns>
        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<MediafulDbContext>();
            context.Database.Migrate();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            // If roles already exist, early return and do not duplicate them.
            if (roleManager.Roles.Count() > 0)
                return;

            // Create roles.
            await roleManager.CreateAsync(new IdentityRole<int>("User"));
            await roleManager.CreateAsync(new IdentityRole<int>("Admin"));
        }
    }
}
