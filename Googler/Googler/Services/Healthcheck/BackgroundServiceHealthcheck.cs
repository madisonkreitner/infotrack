using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Googler.Services
{
    /// <summary>
    /// Examines the Service Health state to determine if the service worker has checked in within the threshold period of time defined through configuration
    /// </summary>
    public class BackgroundServiceHealthcheck : IHealthCheck
    {
        #region Fields
        private readonly ILogger<BackgroundServiceHealthcheck> _logger;
        private readonly IServiceHealthState _serviceHealthState;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="serviceHealthState">Current health state</param>
        public BackgroundServiceHealthcheck(ILogger<BackgroundServiceHealthcheck> logger, IServiceHealthState serviceHealthState)
        {
            try
            {
                _logger = logger;
                _serviceHealthState = serviceHealthState;
                _logger.LogDebug("Initialized {@class}", nameof(BackgroundServiceHealthcheck));
            }
            catch (Exception e)
            {
                _logger!.LogError(e, "Failed to initialize {@class}", nameof(BackgroundServiceHealthcheck));
                throw;
            }
        }
        #endregion

        #region Methods
        /// <inheritdoc />
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            HealthCheckResult results;
            bool healthy = true;

            // Get health of main topic processor
            DateTime lastHealthCheck = _serviceHealthState.GetHealthState(IServiceHealthState.TopicProcessorId.MainTopic);
            healthy &= lastHealthCheck > DateTime.UtcNow.AddSeconds(-60); // Did background worker service checkin within last 60 seconds

            // Get Health of retry topic processor
            lastHealthCheck = _serviceHealthState.GetHealthState(IServiceHealthState.TopicProcessorId.MainTopic);
            healthy &= lastHealthCheck > DateTime.UtcNow.AddSeconds(-60); // Did background worker service checkin within last 60 seconds

            results = healthy ? HealthCheckResult.Healthy("Healthy") : HealthCheckResult.Unhealthy("UnHealthy");
            if (!healthy)
            {
                _logger.LogWarning("{method} {@context} {@results} {@lastHealthCheck}", nameof(CheckHealthAsync), context, results, lastHealthCheck);
            }
            _logger.LogTrace("{method} {context} {results}", nameof(CheckHealthAsync), context, results);
            return Task.FromResult(results);
        }
        #endregion
    }
}