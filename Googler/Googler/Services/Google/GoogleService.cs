using Googler.Controllers;
using Googler.Helpers;
using Googler.Models;
using Googler.Services.Html;
using Microsoft.Extensions.Options;

namespace Googler.Services.Google
{
    public class GoogleService : IGoogleService
    {
        #region Fields
        private readonly HttpClient _httpClient;
        private readonly ILogger<GoogleService> _logger;
        private readonly IOptions<HttpClientConfiguration> _httpConfig;
        private readonly IHtmlService _htmlService;
        #endregion

        #region Properties
        private HttpClient Client => _httpClient;
        private HttpClientConfiguration Options => _httpConfig.Value;
        #endregion

        public GoogleService(ILogger<GoogleService> logger,
                                IHttpClientFactory httpClientFactory,
                                IOptions<HttpClientConfiguration> httpConfig,
                                IHtmlService htmlService)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(GooglerController));
            _httpClient.BaseAddress = new Uri(httpConfig.Value.BaseUrl);
            foreach (KeyValuePair<string, string?> header in httpConfig?.Value.DefaultRequestHeaders ?? Enumerable.Empty<KeyValuePair<string, string?>>())
            {
                _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            _logger = logger;
            _httpConfig = httpConfig ?? throw new InvalidOperationException("null config");
            _htmlService = htmlService;
        }

        public async Task<Statistics> GetQueryStatistics(string query, string keyword)
        {
            string requestUri = $"{Options?.Endpoint}?num=100&q={query}";
            using HttpRequestMessage request = new(HttpMethod.Get, requestUri);

            try
            {
                using HttpResponseMessage response = await Client.SendAsync(request).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    _logger.LogError("{class} Failed to get data from Google. {@requestUri} {@response} {responseBody}", nameof(GooglerController), requestUri, response, errorResponseBody);
                    throw new InvalidOperationException($"Failed to get data from Google. {requestUri} {@response} {errorResponseBody}");
                }
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                List<string> tags = _htmlService.GetATagsFromHtml(responseBody);
                List<string> links = new();
                foreach (string tag in tags) 
                {
                    string l = _htmlService.GetHrefFromATag(tag);
                    if (!string.IsNullOrWhiteSpace(l))
                    {
                        links.Add(l);
                    }
                }
                List<string> domains = new();
                foreach (string link in links)
                {
                    string d = UrlHelper.GetDomainName(link);
                    if (!string.IsNullOrWhiteSpace(d))
                    {
                        domains.Add(d);
                    }
                }
                Statistics s = new();
                return s;
            }
            catch (Exception e)
            {
                _logger.LogError(e, nameof(GooglerController));
                throw;
            }
        }
    }
}
