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

#if !DEBUG
    var config = Config.Default;
    string? logDir = config.Log.Dir;
    if (!string.IsNullOrWhiteSpace(logDir))
    {
        string logPath = Path.Join(logDir, $"{DateTime.Now:yyyy-MM-dd HH.mm.ss}.log");
        conf = conf.WriteTo.File(logPath, config.Log.Level);
    }
#endif

    Log.Logger = conf.CreateLogger();
}

ConfigureLogger();

static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    var exception = (Exception)e.ExceptionObject;
    Log.Fatal(exception, "未知异常，正在退出程序");
}

AppDomain appDomain = AppDomain.CurrentDomain;
appDomain.UnhandledException += OnUnhandledException;

var app = new CommandApp<SyncPixivCommand>();
return await app.RunAsync(args);