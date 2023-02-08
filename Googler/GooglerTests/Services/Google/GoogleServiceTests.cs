using Googler.Models;
using Googler.Services.Google;
using Googler.Services.Html;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GooglerTests.Services.Google
{
    [TestFixture]
    internal class GoogleServiceTests
    {
        private GoogleService _service;
        private Mock<ILogger<GoogleService>> _logger;
        private Mock<IHttpClientFactory> _httpClientFactory;
        private Mock<IOptions<HttpClientConfiguration>> _options;
        private Mock<IHtmlService> _htmlService;
        private Mock<HttpClient> _httpClient;

        [SetUp]
        public void Setup()
        {
            _logger = new();
            _httpClientFactory = new();
            _options = new();
            _htmlService = new();
            _httpClient = new();
            _httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(_httpClient.Object);
            HttpClientConfiguration options = new()
            {
                BaseUrl = "https://www.example.com"
            };

            _options.SetupGet(o => o.Value).Returns(options);
            _service = new(_logger.Object, _httpClientFactory.Object, _options.Object, _htmlService.Object);
        }

        [Test]
        [Category("UnitTest")]
        public void GetSearchResultsTest()
        {
            _htmlService.Setup(s => s.GetSearchResultsFromHtml(It.IsAny<string>())).Returns(new List<string>() { "1", "2" });
            int count = _service.GetSearchResults("123", 123).Result.Count();
            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        [Category("UnitTest")]
        public void GetSearchResultsThrowsTest()
        {
            _htmlService.Setup(s => s.GetSearchResultsFromHtml(It.IsAny<string>())).Throws(new InvalidOperationException());
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.GetSearchResults("123", 123));

            _htmlService.Setup(s => s.GetHrefFromATag(It.IsAny<string>())).Throws(new InvalidOperationException());
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.GetSearchResults("123", 123));
        }
    }
}
