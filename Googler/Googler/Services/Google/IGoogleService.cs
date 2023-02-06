namespace Googler.Services.Google
{
    public interface IGoogleService
    {
        Task<List<string>> GetQueryStatistics(string query, string keyword);
    }
}