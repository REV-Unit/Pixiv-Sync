using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using NHibernate;
using PixivSync;
using PixivSync.Pixiv;
using PixivSync.Pixiv.ApiResponse.GetBookmarksResponse;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

#if DEBUG
HttpClient.DefaultProxy = new WebProxy("127.0.0.1", 8888);
#endif

IConfiguration config = Config.Default;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
#if RELEASE
    .WriteTo.File($"{AppContext.BaseDirectory}/log/{DateTime.Now:yyyy-MM-dd HH.mm.ss}.log")
#endif
    .CreateLogger();

IllustBookmarkInfo[] result = await Account.GetAllBookmarks(Config.MainAccountId, Config.MainAccountCookie);

Log.Information("计算 Delta");

IllustBookmarkInfo[] delta;
using (ISession session = Database.SessionFactory.OpenSession())
{
    delta = result.ExceptBy(session.Query<Illust>().Select(illust => illust.Id), r => Convert.ToInt64(r.id)).ToArray();
}

//IllustBookmarkInfo[] delta= result.ToArray();

Log.Information("Delta {Delta} 个插画", delta.Length);
if (delta.Length == 0)
{
    Log.Information("不进行操作");
    //return 1;
}

Illust[] illusts = await Illust.FromBookmarkInfo(delta);
Database.Merge(illusts);

var storage = Storage.Default;
// storage.ResolveArtistNameChanges();

using (ISession session = Database.SessionFactory.OpenSession())
{
    IQueryable<Illust> availableIllusts = session.Query<Illust>();
    await storage.BeginSaveIllust(availableIllusts);
}

return 0;