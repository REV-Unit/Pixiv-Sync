using NHibernate;
using PixivSync.Pixiv.ApiResponse.GetBookmarksResponse;
using Serilog;

namespace PixivSync.Pixiv;

public class User
{
    public long Id { get; init; }
    public string? Cookie { get; set; }

    private static int DivideCeiling(int x, int y)
    {
        return (x + y - 1) / y;
    }

    public async Task<IllustBookmarkInfo[]> GetAllBookmarks()
    {
        Log.Information("开始获取所有收藏");
        IPixivApi pixivApi = PixivApi.Default;

        List<Task<GetBookmarksResponse>> getBookmarksTasks = new() { pixivApi.GetBookmarks(Id, 0, 100, Cookie) };
        GetBookmarksResponse resp1 = await getBookmarksTasks[0];
        int total = resp1.body.total;

        for (var i = 1; i < DivideCeiling(total, 100); i++)
        {
            getBookmarksTasks.Add(pixivApi.GetBookmarks(Id, i * 100, 100, Cookie));
        }

        await Task.WhenAll(getBookmarksTasks);
        IllustBookmarkInfo[] result = getBookmarksTasks.SelectMany(t => t.Result.body.works).ToArray();
        Log.Information("获取到 {Count} 项", result.Length);
        return result;
    }

    public IAsyncEnumerable<IllustBookmarkInfo> GetAddedBookmarks()
    {
        Log.Information("开始获取添加的收藏");
        using ISession session = Database.SessionFactory.OpenSession();
        HashSet<long> ids = session.Query<Illust>().Select(illust => illust.Id).ToHashSet();
        return EnumerateBookmarks().TakeWhile(bookmarkInfo => !ids.Contains(Convert.ToInt64(bookmarkInfo.id)));
    }

    public async IAsyncEnumerable<IllustBookmarkInfo> EnumerateBookmarks()
    {
        Log.Information("开始枚举收藏");
        IPixivApi pixivApi = PixivApi.Default;

        int total;
        var offset = 0;
        do
        {
            GetBookmarksResponse resp =
                await pixivApi.GetBookmarks(Id, 0, 100, Cookie);
            total = resp.body.total;

            foreach (IllustBookmarkInfo bookmarkInfo in resp.body.works)
            {
                yield return bookmarkInfo;
            }

            offset += resp.body.works.Count;
        } while (offset < total);
    }
}