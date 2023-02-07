namespace Googler.Services.Html
{
    public interface IHtmlService
    {
        List<string> GetSearchResultsFromHtml(string html);
        string GetHrefFromATag(string tag);
    }
}