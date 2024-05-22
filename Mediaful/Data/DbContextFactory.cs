using Microsoft.EntityFrameworkCore;

namespace Mediaful.Data
{
    /// <summary>
    /// Class that generates database contexts.
    /// </summary>
    public class DbContextFactory
    {
        /// <summary>
        /// DbContextOptions to use.
        /// </summary>
        private static DbContextOptions<MediafulDbContext> options = null;

        /// <summary>
        /// Update DbContextOptions connection string.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        public static void SetConnectionString(string connectionString)
        {
            options = new DbContextOptionsBuilder<MediafulDbContext>()
                .UseSqlServer(connectionString)
                .Options;
        }

        /// <summary>
        /// Create new DbContext.
        /// </summary>
        /// <returns>New DbContext.</returns>
        public static MediafulDbContext GetContext()
        {
            return options == null ? null : new MediafulDbContext(options);
        }
    }
}