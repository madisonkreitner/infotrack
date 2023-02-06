using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System;
using Googler.Services;
using Googler.Models;
using Googler.Services.Html;
using Googler.Services.Google;

namespace Googler
{
    /// <summary>
    /// Startup Class
    /// </summary>
    public class Startup
    {
        private readonly string _ServiceTitle = "Googler Service";
        private readonly string _ServiceDesc = "Manages various functions of scraping google searches.";
        private readonly WaitHandle _waitHandler = new ManualResetEvent(false);

        /// <summary>
        ///  Constructor
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Services Collection</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_waitHandler);

            services.AddControllers();
            services.AddSwaggerDocument(o =>
            {
                o.Title = _ServiceTitle;
                o.Description = _ServiceDesc;
            });

            services.Configure<HttpClientConfiguration>(Configuration.GetSection(nameof(HttpClientConfiguration)));

            AddHealthcheck(services);

            services.AddHttpClient();

            services.AddScoped<IHtmlService, HtmlService>();
            services.AddScoped<IGoogleService, GoogleService>();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application Builder</param>
        /// <param name="env">WebHostEnvironment</param>
        /// <param name="logger">Logger</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // UNCOMMENT to enable HTTPS redirection
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                //Map healthcheck endpoint. Will return either 200 or 503 depending on state
                endpoints.MapHealthChecks("/health", new HealthCheckOptions() { AllowCachingResponses = false });
            });

            ConfigureOpenApi(app);

            ConfigureRevProxyForwarding(app, logger);

            logger.LogDebug("Finished running Startup::Configure()");

            // WARNING: Following MUST be last statement in the function as this will release the background threads to begin processing
            ((ManualResetEvent)_waitHandler).Set();
        }

        /// <summary>
        /// Sets up appropriate forwarding of headers to handle endpoints that might be sitting behind reverse proxies (kubernetes services experience this level of indirection)
        /// </summary>
        /// <param name="app">Application Builder</param>
        /// <param name="logger">Logger</param>
        static private void ConfigureRevProxyForwarding(IApplicationBuilder app, ILogger<Startup> logger)
        {
            app.UseForwardedHeaders();
            app.Use(async (context, next) =>
            {
                logger.LogTrace("Request Method: {Method}", context.Request.Method);
                logger.LogTrace("Request Scheme: {Scheme}", context.Request.Scheme);
                logger.LogTrace("Request Path: {Path}", context.Request.Path);

                if (context.Request.Headers.ContainsKey("X-Forwarded-Prefix"))
                {
                    context.Request.PathBase = context.Request.Headers["X-Forwarded-Prefix"].First();
                }
                logger.LogTrace("Request Path Base: {Path}", context.Request.PathBase);

                await next();
            });
        }

        /// <summary>
        /// Setup endpoint for serving up OpenApi specification
        /// </summary>
        /// <param name="app">Application Builder</param>
        static private void ConfigureOpenApi(IApplicationBuilder app)
        {
            const string swaggerJson = "/swagger/v1/swagger.json";
            app.UseOpenApi(o =>
            {
                o.DocumentName = "v1";
                o.Path = swaggerJson;
                o.PostProcess = (document, request) =>
                {
                    if (!new[] { "X-Forwarded-Prefix" }.All(k => request.Headers.ContainsKey(k)))
                    {
                        return;
                    }
                    document.BasePath = request.Headers["X-Forwarded-Prefix"].First();
                };
            });
            app.UseSwaggerUi3(o =>
            {
                o.Path = "/swagger";
                // The header X-Forwarded-Prefix is set in the reverse proxy
                o.TransformToExternalPath = (internalUiRoute, request) =>
                {
                    string path = request.Headers.ContainsKey("X-Forwarded-Prefix") ? request.Headers["X-Forwarded-Prefix"].First() : "";
                    path += internalUiRoute;
                    return path;
                };
            });
        }

        /// <summary>
        /// Sets up the services and configuration objects for performing health checks against background worker services
        /// </summary>
        /// <param name="services">Service Collection</param>
        static private void AddHealthcheck(IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IServiceHealthState, ServiceHealthState>();
            services
                .AddHealthChecks()
                .AddCheck<BackgroundServiceHealthcheck>("Service worker checkin");
        }

    }
}
