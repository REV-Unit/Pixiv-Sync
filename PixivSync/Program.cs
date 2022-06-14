using System.IO;
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
    .MinimumLevel.Verbose()
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

Log.Information($"Delta [{delta.Length}] 个插画");

//IllustBookmarkInfo[] delta = result.ToArray();

Illust[] illusts = await Illust.FromBookmarkInfo(delta);
illusts = Database.Merge(illusts);

Log.Information("准备发送下载到 aria2");
var aria2 = new Aria2();
Aria2RpcResponse[] responses = illusts.AsParallel().WithDegreeOfParallelism(32).SelectMany(illust => illust.Pages,
    (illust, illustPage) => aria2.AddUri(illustPage.Original,
        new AddUriParams
        {
            SaveDir = Path.Join(config["Download:Root"], $"{illust.Artist?.Name ?? ""}_{illust.Artist?.Id ?? 0}"),
            Referer = "https://www.pixiv.net",
            Cookie = Config.AuxAccountCookie,
            CondictionalGet = true,
            AutoFileRenaming = true,
            RetryWait = 5
        }).Result).ToArray();
Log.Information("已全部发送");