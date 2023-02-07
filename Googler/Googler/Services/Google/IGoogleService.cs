using Googler.Models;

namespace Googler.Services.Google
{
    public interface IGoogleService
    {
        Task<Statistics> GetQueryStatistics(string query, string keyword);
    }
}