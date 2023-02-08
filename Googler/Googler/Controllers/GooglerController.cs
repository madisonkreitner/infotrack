using Googler.Models;
using Googler.Services.Google;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Googler.Controllers
{
    [ApiController]
    [EnableCors()]
    [Route("/")]
    public class GooglerController : ControllerBase
    {
        private readonly ILogger<GooglerController> _logger;
        private readonly IGoogleService _googleService;

        public GooglerController(ILogger<GooglerController> logger, IGoogleService googleService)
        {
            _logger = logger;
            _googleService = googleService;
        }

        /// <summary>
        /// Find statistics about a google search
        /// </summary>
        /// <remarks>Returns an html string</remarks>
        /// <response code="200">Successful operation.</response>
        /// <response code="400">Unsuccessful operation.</response>
        [HttpGet]
        [Route("/searchResults")]
        [ProducesResponseType(statusCode: 200, type: typeof(string))]
        [ProducesResponseType(statusCode: 400)]
        public async Task<IEnumerable<SearchResult>> GetSearchResults([FromQuery]string queryString, [FromQuery]int count = 100)
        {
            try
            {
                return await _googleService.GetSearchResults(queryString, count);
            }
            catch (Exception e)
            {
                _logger.LogError(e, nameof(GooglerController));
                throw;
            }
        }
    }
}