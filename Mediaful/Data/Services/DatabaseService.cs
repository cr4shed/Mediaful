using Mediaful.Data.Models;
using Mediaful.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Mediaful.Data.Services
{
    /// <summary>
    /// Database service class which handles all of the EFCore database queries.
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        /// <summary>
        /// Generic method to get a collection of records from any DbSet. 
        /// Capable of filtering values based on a predicate and eager loading other entities.
        /// </summary>
        /// <typeparam name="T">Entity which specifies the DbSet to query.</typeparam>
        /// <param name="predicate">Function that tests each element for a condition.</param>
        /// <param name="includes">Specifies related entities to include in the query results.</param>
        /// <returns>Collection of records of type T.</returns>
        public async Task<List<T>> GetRecords<T>(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes) where T : class
        {
            using var context = DbContextFactory.GetContext();
            try
            {
                // Target DbSet based on provided type.
                IQueryable<T> query = context.Set<T>();

                // Handle eager loading includes.
                if (includes != null && includes.Length > 0)
                {
                    foreach (Expression<Func<T, object>> include in includes)
                    {
                        query = query.Include(include);
                    }
                }

                // Filter.
                if (predicate != null)
                {
                    query = query.Where(predicate);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Generic method to get a single record from any DbSet.
        /// Capable of filtering values based on a predicate and eager loading other entities.
        /// </summary>
        /// <typeparam name="T">Entity which specifies the DbSet to query.</typeparam>
        /// <param name="predicate">Function that tests each element for a condition.</param>
        /// <param name="includes">Specifies related entities to include in the query results.</param>
        /// <returns>Single record of type T or default if record cannot be found.</returns>
        public async Task<T> GetRecord<T>(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes) where T : class
        {
            using var context = DbContextFactory.GetContext();
            try
            {
                // Target DbSet based on provided type.
                IQueryable<T> query = context.Set<T>();

                // Handle eager loading includes.
                if (includes != null && includes.Length > 0)
                {
                    foreach (Expression<Func<T, object>> include in includes)
                    {
                        query = query.Include(include);
                    }
                }

                // Filter.
                if (predicate != null)
                {
                    query = query.Where(predicate);
                }

                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Generic method to get a single record from any DbSet by using a record ID.
        /// </summary>
        /// <typeparam name="T">Entity which specifies the DbSet to query.</typeparam>
        /// <param name="recordId">Record ID.</param>
        /// <returns>Single record with an ID of recordId or null if no record found.</returns>
        public async Task<T> GetRecord<T>(int recordId) where T : class
        {
            using var context = DbContextFactory.GetContext();

            return await context.Set<T>().FindAsync(recordId);
        }

        /// <summary>
        /// Generic method to insert a new record.
        /// </summary>
        /// <typeparam name="T">Entity which specifies the DbSet to query.</typeparam>
        /// <param name="record">New record to insert.</param>
        /// <returns>Task result which contains the number of state entries written to the database.</returns>
        public async Task<int> Insert<T>(T record) where T : class
        {
            using var context = DbContextFactory.GetContext();
            try
            {
                context.Add(record);
                return await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Generic method to update an existing record.
        /// </summary>
        /// <typeparam name="T">Entity which specifies the DbSet to query.</typeparam>
        /// <param name="record">Updated record.</param>
        /// <returns>Task result which contains the number of state entries written to the database.</returns>
        public async Task<int> Update<T>(T record) where T : class
        {
            using var context = DbContextFactory.GetContext();
            try
            {
                context.Update(record);
                return await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Generic method to delete an existing record.
        /// </summary>
        /// <typeparam name="T">Entity which specifies the DbSet to query.</typeparam>
        /// <param name="record">Record to delete.</param>
        /// <returns>Task result which contains the number of state entries written to the database.</returns>
        public async Task<int> Delete<T>(T record) where T : class
        {
            using var context = DbContextFactory.GetContext();
            try
            {
                context.Remove(record);
                return await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Generic method to delete an existing record.
        /// </summary>
        /// <typeparam name="T">Entity which specifies the DbSet to query.</typeparam>
        /// <param name="recordId">ID of the record to delete.</param>
        /// <returns>Task result which contains the number of state entries written to the database.</returns>
        public async Task<int> Delete<T>(int recordId) where T : class
        {
            using var context = DbContextFactory.GetContext();
            try
            {
                var record = await context.FindAsync<T>(recordId);
                return await Delete<T>(record);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Method to get a specified number of notification records with a possible offset.
        /// </summary>
        /// <param name="userId">Id of the authenticated user.</param>
        /// <param name="count">Number of records to take.</param>
        /// <param name="offset">Number of records to skip.</param>
        /// <returns>List of notification records.</returns>
        public async Task<List<Notification>> GetNotifications(int userId, int count, int offset)
        {
            using var context = DbContextFactory.GetContext();
            try
            {
                // Fetch notifications where from all users except the authenticated user.
                // Uses Skip() and Take() to prevent scaling issues as notification volume increases.
                return await context.Notifications.Where(n => n.SenderId != userId)
                    .OrderByDescending(n => n.NotificationId)
                    .Skip(offset)
                    .Take(count)
                    .ToListAsync();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Method to get a specified number of user records which contain a substring.
        /// </summary>
        /// <param name="searchString">Substring to search usernames for.</param>
        /// <param name="count">Number of records to take.</param>
        /// <param name="offset">Number of records to skip.</param>
        /// <param name="userId">Id of the authenticated user (if applicable). Default is -1 which indicates the user is not authenticated.</param>
        /// <returns>List of user records.</returns>
        public async Task<List<ApplicationUser>> GetUsers(string searchString, int count, int offset, int userId = -1)
        {
            using var context = DbContextFactory.GetContext();
            try
            {
                IQueryable<ApplicationUser> query = context.Users;

                // If a search string is provided, find all usernames that contain it.
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    query = query.Where(u => u.NormalizedUserName.Contains(searchString.ToUpper()));
                }

                // If user is authenticated do not display themselves in the search.
                if (userId != -1)
                {
                    query = query.Where(u => u.Id != userId);
                }

                return await query.Skip(offset).Take(count).ToListAsync();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Method to log watch data into a user's collection.
        /// </summary>
        /// <param name="userId">Id of the user adding the watch data.</param>
        /// <param name="data">Watch data to add.</param>
        /// <returns>Task.</returns>
        public async Task LogWatchData(int userId, WatchData data)
        {
            using var context = DbContextFactory.GetContext();

            try
            {
                // Check if the user has an existing log for this title.
                WatchData existing = await GetRecord<WatchData>(wd => wd.UserId == userId && wd.Title == data.Title);

                // If user has an existing log, update it to reflect the new changes.
                if (existing != null)
                {
                    existing.Status = data.Status;
                    existing.Rating = data.Rating;
                    existing.IsFavorite = data.IsFavorite;
                    existing.IsTvShow = data.IsTvShow;
                    existing.TvSeason = data.TvSeason;
                    existing.TvEpisode = data.TvEpisode;
                    existing.LastWatched = data.FirstWatchDate;

                    // If the status is Watched, update watched properties.
                    if (existing.Status == (int)StatusEnum.Watched)
                    {
                        existing.LastWatched = DateTime.Now;
                        existing.TimesWatched++;
                    }

                    // If this is the first watch, update the date.
                    if (existing.FirstWatchDate == null)
                    {
                        existing.FirstWatchDate = existing.LastWatched;
                    }

                    context.Update(existing);
                }
                // Create new log.
                else
                {
                    // If the status is Watched, update watched properties.
                    if (data.Status == (int)StatusEnum.Watched)
                    {
                        data.LastWatched = DateTime.Now;
                        data.FirstWatchDate = data.LastWatched;
                        data.TimesWatched++;
                    }
                    context.WatchData.Add(data);
                }
                data.UserId = userId;
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// Method to get watch statistics.
        /// </summary>
        /// <returns>Dictionary which contains the title and its total watched count.</returns>
        public async Task<Dictionary<string, int>> GetWatchStats()
        {
            using var context = DbContextFactory.GetContext();

            try
            {
                Dictionary<string, int> dict = new();

                // Query the database for the count of the top 5 WATCHED titles.
                // Query results in dynamic { Name, Count } objects.
                var query = await context.WatchData
                    .Where(wd => wd.Status == (int)StatusEnum.Watched)
                    .GroupBy(wd => wd.Title)
                    .Select(x => new { Name = x.Key, Count = x.Count() })
                    .OrderByDescending(kv => kv.Count)
                    .Take(5)
                    .ToListAsync();

                // Store the results in the dictionary as title, count.
                foreach (var item in query)
                {
                    dict.Add(item.Name, item.Count);
                }

                return dict;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
