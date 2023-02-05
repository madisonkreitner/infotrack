using Googler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InternalProductMappingService
{
    /// <summary>
    /// Main Program
    /// </summary>
    public class Program
    {
        static private readonly string _appName = "Googler";
        static private readonly string _filenameFriendlyApp = string.Join('_', _appName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        static private readonly string _environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        static private readonly int _exposedPort = 50342;

        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args">Commandline Argument</param>
        public static void Main(string[] args)
        {
            try
            {
                //There is a bit of a chicken/egg issue with start-up and log. DI (with env, config settings)
                //Using console for main start-up. NLog config is introduced when the Host builder completes with
                //full logging
                Console.WriteLine($"Starting {_appName}");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                //NLog: catch setup errors
                Console.WriteLine("Stopped program because of following exception");
                Console.WriteLine(e.ToString());
                throw;
            }
            finally
            {
                LogManager.GetCurrentClassLogger().Info($"Shutdown {_appName}");

                // Ensure to flush and stop internal logging threads before exiting
                LogManager.Flush();
                LogManager.Shutdown();
            }
        }

        /// <summary>
        /// Web Host Builder
        /// </summary>
        /// <param name="args">Commandline Arguments</param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(new[] { $"http://*:{_exposedPort}" });
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

                    // Allows for environment specific configuration overrides.
                    // Useful when deploying application into Kubernetes where
                    // volumes can be configured to link Kubernetes ConfigMap objects 
                    // into files in a local directory
                    string filePath = $"/config/{_filenameFriendlyApp}-v1.json";
                    Console.WriteLine($"Optional \"{filePath}\" config file exists? => {File.Exists(filePath)}");
                    config.AddJsonFile(filePath, optional: true, reloadOnChange: true);

                    filePath = "/secrets/shared/connectionstrings.json";
                    Console.WriteLine($"Optional \"{filePath}\" config file exists? => {File.Exists(filePath)}");
                    config.AddJsonFile(filePath, optional: true, reloadOnChange: true);

                    filePath = $"/secrets/{_filenameFriendlyApp}-v1.json";
                    Console.WriteLine($"Optional  \"{filePath}\" config file exists? => {File.Exists(filePath)}");
                    config.AddJsonFile(filePath, optional: true, reloadOnChange: true);

                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);

                    List<string> nlogConfigFiles = new List<string>()
                            {
                                // Set up log configuration precedence...
                                $"/config/{_filenameFriendlyApp}-nlog.config",
                                "/config/shared/nlog.config",
                                string.IsNullOrWhiteSpace(_environment) ? "./nlog.Development.config" : $"./nlog.{_environment}.config",
                                "./nlog.config"
                            };
                    Console.WriteLine($"Searching for nlog.config files in the following priority locations: [{string.Join(", ", nlogConfigFiles.Select(x => $"\"{x}\""))}]");
                    string? nlogConfigFile = nlogConfigFiles.FirstOrDefault(f => File.Exists(f));
                    if (string.IsNullOrEmpty(nlogConfigFile))
                    {
                        // Look for first config file found starting from the top of the drive
                        string[] files = Directory.GetFiles("/", "nlog*.config", SearchOption.AllDirectories);
                        if (files.Length == 0)
                        {
                            throw new FileNotFoundException($"Unable to find the file nlog.config anywhere on disk");
                        }
                        else
                        {
                            nlogConfigFile = files[0];
                            Console.WriteLine($"Found an nlog.config file at the following location: {nlogConfigFile}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Found an nlog.config file at the following location: {nlogConfigFile}");
                    }
                    //full path needed NLog to find config in container
                    logging.AddNLog(Path.GetFullPath(nlogConfigFile));
                })
              .UseNLog();
    }
}