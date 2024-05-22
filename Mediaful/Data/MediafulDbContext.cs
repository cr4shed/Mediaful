using Mediaful.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Mediaful.Data
{
    /// <summary>
    /// Database context class.
    /// Inherits IdentityDbContext for use within the scaffolded Identity pages.
    /// </summary>
    public class MediafulDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        /// <summary>
        /// Constructor. Calls base Identity constructor.
        /// </summary>
        /// <param name="options">DbContextOptions.</param>
        public MediafulDbContext(DbContextOptions<MediafulDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// WatchData DbSet which stores the WatchData.
        /// </summary>
        public virtual DbSet<WatchData> WatchData { get; set; }

        /// <summary>
        /// Notifications DbSet which stores the Notifications.
        /// </summary>
        public virtual DbSet<Notification> Notifications { get; set; }

        /// <summary>
        /// NotificationReports DbSet which stores the NotificationReports.
        /// </summary>
        public virtual DbSet<NotificationReport> NotificationReports { get; set; }

        /// <summary>
        /// FeaturedTitles DbSet which stores the FeaturedTitles.
        /// </summary>
        public virtual DbSet<FeaturedTitle> FeaturedTitles { get; set; }

        /// <summary>
        /// On model creating method that calls the base Identity OnModelCreating method.
        /// </summary>
        /// <param name="modelBuilder">ModelBuilder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}