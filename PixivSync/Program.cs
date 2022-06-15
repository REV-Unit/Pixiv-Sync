using NHibernate;
using PixivSync;
using PixivSync.Pixiv;
using PixivSync.Pixiv.ApiResponse.GetBookmarksResponse;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

#if DEBUG
// Dammit don't use the environment proxy! (It does not support bypass IP pattern.) https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.defaultproxy?view=net-6.0
Environment.SetEnvironmentVariable("HTTP_PROXY", string.Empty);
Environment.SetEnvironmentVariable("HTTPS_PROXY", string.Empty);
#endif

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
#if RELEASE
    .WriteTo.File($"{AppContext.BaseDirectory}/log/{DateTime.Now:yyyy-MM-dd HH.mm.ss}.log")
#endif
    .CreateLogger();

var user = new User { Id = Config.MainAccountId, Cookie = Config.MainAccountCookie };
IllustBookmarkInfo[] bookmarkInfos = await user.GetAddedBookmarks();

if (bookmarkInfos.Length == 0)
{
    Log.Information("不进行操作");
    return 1;
}

Illust[] illusts = await Illust.FromBookmarkInfo(bookmarkInfos);
await Database.Merge(illusts);

var storage = Storage.Default;
storage.ResolveArtistNameChanges();

using (ISession session = Database.SessionFactory.OpenSession())
{
    IQueryable<Illust> availableIllusts = session.Query<Illust>();
    await storage.BeginSaveIllust(availableIllusts);
}

return 0;