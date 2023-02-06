using System.Text;
using YamlDotNet.Core.Tokens;

namespace Googler.Helpers
{
    public class UrlHelper
    {
        public static string GetDomainName(string url)
        {
            StringBuilder s = new();
            int i = 0;
            string http = "http://";
            string https = "https://";
            while (i < url.Length)
            {
                char c = url[i];
                if (i + http.Length < url.Length && url.Substring(i, http.Length) == http)
                {
                    i += http.Length;
                    while (i < url.Length && url[i] != '/')
                    {
                        s.Append(url[i]);
                        i++;
                    }
                }
                else if (i + https.Length < url.Length && url.Substring(i, https.Length) == https)
                {
                    i += https.Length;
                    while (i < url.Length && url[i] != '/')
                    {
                        s.Append(url[i]);
                        i++;
                    }
                }
                i++;
            }
            return s.ToString();
        }
    }
}
