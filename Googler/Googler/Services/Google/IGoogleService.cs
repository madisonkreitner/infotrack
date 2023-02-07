using Googler.Models;

namespace Googler.Services.Google
{
    public interface IGoogleService
    {
        Task<IEnumerable<SearchResult>> GetSearchResults(string query, int numResults);
    }
}