namespace Googler.Services.Html
{
    public interface IHtmlService
    {
        List<string> GetATagsFromHtml(string html);
        string GetHrefFromATag(string tag);
    }
}