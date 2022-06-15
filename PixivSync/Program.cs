using NHibernate;
using PixivSync;
using PixivSync.Pixiv;
using PixivSync.Pixiv.ApiResponse.GetBookmarksResponse;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

#if DEBUG
//HttpClient.DefaultProxy = new WebProxy("127.0.0.1", 8888);
#endif

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
#if RELEASE
    .WriteTo.File($"{AppContext.BaseDirectory}/log/{DateTime.Now:yyyy-MM-dd HH.mm.ss}.log")
#endif
    .CreateLogger();

IllustBookmarkInfo[] bookmarkInfos = await Database.GetBookmarksToProcess(true);

if (bookmarkInfos.Length == 0)
{
    Log.Information("不进行操作");
    return 1;
}

Illust[] illusts = await Illust.FromBookmarkInfo(bookmarkInfos);
Database.Merge(illusts);

var storage = Storage.Default;
storage.ResolveArtistNameChanges();

using (ISession session = Database.SessionFactory.OpenSession())
{
    IQueryable<Illust> availableIllusts = session.Query<Illust>();
    await storage.BeginSaveIllust(availableIllusts);
}

return 0;