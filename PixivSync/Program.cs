using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using NHibernate;
using PixivSync;
using PixivSync.Pixiv;
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

// IllustBookmarkInfo[] result = await Account.GetAllBookmarks(Config.MainAccountId, Config.MainAccountCookie);
//
// Log.Information("计算 Delta");
//
// IllustBookmarkInfo[] delta= result.ToArray();
//
// Log.Information($"Delta [{delta.Length}] 个插画");
//
// //IllustBookmarkInfo[] delta = result.ToArray();
//
// Illust[] illusts = await Illust.FromBookmarkInfo(delta);
// illusts = Database.Merge(illusts);

var storage = Storage.Default;
storage.ResolveArtistNameChanges();

Illust[] availableIllusts;
using (ISession session = Database.SessionFactory.OpenSession())
{
    availableIllusts = session.Query<Illust>().Where(illust => !illust.Deleted).ToArray();
}

await storage.BeginSaveIllust(availableIllusts);