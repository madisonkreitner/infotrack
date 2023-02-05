namespace Googler.Models
{
    public class HttpClientConfiguration
    {
        /// <summary>
        /// Base url for the http client.
        /// </summary>
        public string BaseUrl { get; set; } = "https://www.google.com";

        /// <summary>
        /// Headers for the HttpRequestMessage.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string?>> DefaultRequestHeaders { get; set; } = Enumerable.Empty<KeyValuePair<string, string?>>();

        /// <summary>
        /// The endpoint for the particular call.
        /// </summary>
        public string? Endpoint { get; set; }
    }
}
