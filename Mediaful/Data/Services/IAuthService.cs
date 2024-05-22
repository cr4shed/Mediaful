namespace Mediaful.Data.Services
{
    /// <summary>
    /// Authentication service interface which handles fetching information about
    /// an authenticated user using the application.
    /// </summary>
    public interface IAuthService
    {
        Task<int> GetUserId();
        Task<string> GetUserName();
    }
}