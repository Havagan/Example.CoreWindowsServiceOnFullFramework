using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Havagan.Example.CoreWindowsServiceOnFullFramework;

/// <summary>
/// Main entry point for the Service.
/// </summary>
/// <remarks>
/// Project can be built as "Exe" OutputType and rely on the application's run context to determine 
/// Console vs WindowsService context.
/// </remarks>
public class Program
{
    internal const string ApplicationName = "Havagan.Example.CoreWindowsServiceOnFullFramework";

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    /// <remarks>
    /// Earlier versions of C# (C# 7.1) do not support async Main methods.
    /// If the Main method doesn't support async the host.Run() method should be called instead of await host.RunAsync().
    /// </remarks>
    public static async Task Main(string[] args)
    {
        /*
         * ************************************************************
         * Bootstrap the logger.
         * ************************************************************
         */

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .Enrich.WithGlobalProperties()
            .CreateLogger();

        try
        {
            /*
             * ************************************************************
             * Configure the host.
             * ************************************************************
             */

            var host = Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                .UseWindowsService(options =>
                {
                    options.ServiceName = nameof(WorkerService);
                })
                .ConfigureAppConfiguration((ctx, builder) =>
                {
                    builder.AddEnvironmentVariables();
                    builder.AddCommandLine(args);
                    if (ctx.HostingEnvironment.IsDevelopment())
                    {
                        builder.AddUserSecrets<Program>(optional: true);
                    }
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddSerilog();
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<WorkerService>();
                })
                .UseSerilog((context, services, configuration) =>
                {
                    configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services)
                        .Enrich.WithGlobalProperties();
                })
                .Build();

            Log.Logger.Debug("Application configuration completed.");

            /*
             * ************************************************************
             * Execute the service.
             * ************************************************************
             */

            // Use host.Run() or await host.RunAsync() depending on the C# version being used.
            await host.RunAsync();
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
        {
            Log.Logger.Warning("Application execution was cancelled.");
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Unhandled error thrown while running the application.");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
