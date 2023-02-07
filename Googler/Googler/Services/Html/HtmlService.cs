using System.Text;

namespace Googler.Services.Html
{
    public class HtmlService : IHtmlService
    {
        private const string _linkIndicator = "kCrYT\"><a";
        private const string _div = "<div";
        private const string _closingDiv = "</div>";
        private const string _atag = "<a ";
        public HtmlService() { }

        public List<string> GetSearchResultsFromHtml(string html)
        {
            // after this indicator, we hit the search section
            List<string> result = new();
            int i = 0;
            bool foundLink = false;
            int level = 0;
            while (i < html.Length)
            {
                StringBuilder s = new();
                char c = html[i];

                if (i + _linkIndicator.Length < html.Length && html.Substring(i, _linkIndicator.Length) == _linkIndicator)
                {
                    // found a div containing a link
                    foundLink = true;

                    // go to the beginning of the a tag
                    i += _linkIndicator.Length - 3;
                }
                else if (foundLink)
                {
                    if (i + _atag.Length < html.Length && html.Substring(i, _atag.Length) == _atag)
                    {
                        // found an atag, skip it
                        i += _atag.Length;
                        s.Append("<a ");

                        // get the rest of the tag
                        while (i < html.Length && html[i] != '>')
                        {
                            s.Append(html[i]);
                            i++;
                        }
                        s.Append('>');

                        // save it and reset the string builder
                        result.Add(s.ToString());
                        s.Clear();
                        foundLink = false;
                    }
                }
                // go to the next character
                i++;
            }

            return result;
        }

        public string GetHrefFromATag(string tag)
        {
            StringBuilder s = new();
            int i = 0;
            string href = "href=\"";
            while (i < tag.Length)
            {
                if (i + href.Length < tag.Length && tag.Substring(i, href.Length) == href)
                {
                    i += href.Length;
                    while (i < tag.Length && tag[i] != '\"')
                    {
                        s.Append(tag[i]);
                        i++;
                    }
                }
                i++;
            }

            return s.ToString();
        }
    }
}
