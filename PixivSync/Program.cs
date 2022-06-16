using PixivSync;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Spectre.Console.Cli;

// Dammit don't use the environment proxy! (It does not support bypass IP pattern.) https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.defaultproxy?view=net-6.0
Environment.SetEnvironmentVariable("HTTP_PROXY", string.Empty);
Environment.SetEnvironmentVariable("HTTPS_PROXY", string.Empty);

static void ConfigureLogger()
{
    LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console(theme: AnsiConsoleTheme.Code);
    var config = Config.Default;
    if (!string.IsNullOrWhiteSpace(config.Log.Path))
    {
        loggerConfiguration = loggerConfiguration.WriteTo.File(config.Log.Path, config.Log.Level);
    }

    Log.Logger = loggerConfiguration.CreateLogger();
}

ConfigureLogger();

var app = new CommandApp<SyncPixivCommand>();
return await app.RunAsync(args);