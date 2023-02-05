using Googler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Net.Http;
using System.Text;

namespace Googler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GooglerController : ControllerBase
    {
        private readonly ILogger<GooglerController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<HttpClientConfiguration> _httpConfig;
        private readonly HttpClient _httpClient;

        #region Properties
        private HttpClient Client => _httpClient;
        #endregion

        public GooglerController(ILogger<GooglerController> logger, IHttpClientFactory httpClientFactory, IOptions<HttpClientConfiguration> httpConfig)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _httpConfig = httpConfig;
            _httpClient = httpClientFactory.CreateClient(nameof(GooglerController));
            _httpClient.BaseAddress = new Uri(httpConfig.Value.BaseUrl);
            foreach (KeyValuePair<string, string?> header in httpConfig?.Value.DefaultRequestHeaders ?? Enumerable.Empty<KeyValuePair<string, string?>>())
            {
                _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        /// <summary>
        /// Find statistics about a google search
        /// </summary>
        /// <remarks>Returns an html string</remarks>
        /// <response code="200">Successful operation.</response>
        /// <response code="400">Unsuccessful operation.</response>
        [HttpGet]
        [Route("/querystatistics")]
        [ProducesResponseType(statusCode: 200, type: typeof(string))]
        [ProducesResponseType(statusCode: 400)]
        public async Task<string> GetQueryStatistics()
        {
            string requestUri = $"{_httpConfig.Value?.Endpoint}?num=100&q=efiling+integration";
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
                return responseBody;
            }
            catch (Exception e)
            {
                _logger.LogError(e, nameof(GooglerController));
                throw;
            }
        }

        /// <summary>
        /// Gets html code
        /// </summary>
        /// <remarks>Returns an html string</remarks>
        /// <response code="200">Successful operation.</response>
        /// <response code="400">Unsuccessful operation.</response>
        [HttpGet]
        [Route("/html")]
        [ProducesResponseType(statusCode: 200, type: typeof(string))]
        [ProducesResponseType(statusCode: 400)]
        public async Task<HtmlDocument> GetExampleDotCom()
        {
            string requestUri = $"http://www.example.com";
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

                HtmlDocument document = new(responseBody);
                document.LoadElements();
                return document;
            }
            catch (Exception e)
            {
                _logger.LogError(e, nameof(GooglerController));
                throw;
            }
        }
    }
}