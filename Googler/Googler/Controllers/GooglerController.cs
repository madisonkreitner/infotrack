using Googler.Models;
using Googler.Services.Google;
using Microsoft.AspNetCore.Mvc;

namespace Googler.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        [Route("/querystatistics")]
        [ProducesResponseType(statusCode: 200, type: typeof(string))]
        [ProducesResponseType(statusCode: 400)]
        public async Task<Statistics> GetQueryStatistics()
        {
            try
            {
                return await _googleService.GetQueryStatistics("efiling+integration","www.infotrack.com");
            }
            catch (Exception e)
            {
                _logger.LogError(e, nameof(GooglerController));
                throw;
            }
        }
    }
}