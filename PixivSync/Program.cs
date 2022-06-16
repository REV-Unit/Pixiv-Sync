using System.IO;
using PixivSync;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Spectre.Console.Cli;

// Dammit don't use the environment proxy! (It does not support bypass IP pattern.) https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.defaultproxy?view=net-6.0
Environment.SetEnvironmentVariable("HTTP_PROXY", string.Empty);
Environment.SetEnvironmentVariable("HTTPS_PROXY", string.Empty);

static void ConfigureLogger()
{
    LoggerConfiguration conf = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console(theme: AnsiConsoleTheme.Code);
    var config = Config.Default;
    string? logDir = config.Log.Dir;
    if (!string.IsNullOrWhiteSpace(logDir))
    {
        string logPath = Path.Join(logDir, $"{DateTime.Now:yyyy-MM-dd HH.mm.ss}");
        conf = conf.WriteTo.File(logPath, config.Log.Level);
    }

    Log.Logger = conf.CreateLogger();
}

ConfigureLogger();

var app = new CommandApp<SyncPixivCommand>();
return await app.RunAsync(args);