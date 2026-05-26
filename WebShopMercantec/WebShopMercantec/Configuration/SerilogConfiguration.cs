using Serilog;
using Serilog.Events;

namespace WebShopMercantec.Configuration;

public static class SerilogConfiguration
{
    // sets up global logging
    public static void ConfigureSerilog()
    {
        Log.Logger = new LoggerConfiguration()
            // base log level
            .MinimumLevel.Information()
            
            // reduce noise from framework logs
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            
            // add context metadata
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            
            // console for dev
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            
            // file logging with daily rotation
            .WriteTo.File(
                path: "logs/webshop-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                retainedFileCountLimit: 30, // keep 30 days
                fileSizeLimitBytes: 10 * 1024 * 1024, // 10MB limit
                rollOnFileSizeLimit: true)
            
            .CreateLogger();
    }
}
