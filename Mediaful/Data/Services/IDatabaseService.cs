using Mediaful.Data.Models;
using System.Linq.Expressions;

namespace Mediaful.Data.Services
{
    /// <summary>
    /// Database service interface which handles all of the EFCore database queries.
    /// </summary>
    public interface IDatabaseService
    {

        Task<int> Delete<T>(int recordId) where T : class;
        Task<int> Delete<T>(T record) where T : class;
        Task<List<Notification>> GetNotifications(int userId, int count, int offset);
        Task<T> GetRecord<T>(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes) where T : class;
        Task<T> GetRecord<T>(int recordId) where T : class;
        Task<List<T>> GetRecords<T>(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes) where T : class;
        Task<List<ApplicationUser>> GetUsers(string searchString, int count, int offset, int userId = -1);
        Task<Dictionary<string, int>> GetWatchStats();
        Task<int> Insert<T>(T record) where T : class;
        Task LogWatchData(int userId, WatchData data);
        Task<int> Update<T>(T record) where T : class;
    }
}