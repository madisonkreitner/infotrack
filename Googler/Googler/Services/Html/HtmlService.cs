using System.Text;

namespace Googler.Services.Html
{
    public class HtmlService : IHtmlService
    {
        public HtmlService() { }

        public List<string> GetATagsFromHtml(string html)
        {
            List<string> result = new();
            int i = 0;
            while (i < html.Length)
            {
                StringBuilder s = new();
                char c = html[i];
                if (c == '<')
                {
                    // increment and check for the a tag
                    i++;
                    // check for a
                    if (i < html.Length && html[i] == 'a')
                    {
                        i++;
                        // check for space following a
                        if (i < html.Length && html[i] == ' ')
                        {
                            s.Append("<a ");
                            // get the rest of the tag
                            while (i < html.Length && html[i] != '>')
                            {
                                s.Append(html[i]);
                                i++;
                            }
                            s.Append('>');
                            result.Add(s.ToString());
                            s.Clear();
                        }
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
