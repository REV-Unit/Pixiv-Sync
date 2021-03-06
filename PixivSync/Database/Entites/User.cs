using JetBrains.Annotations;
using NHibernate;
using PixivSync.Pixiv;
using PixivSync.Pixiv.ApiResponse.GetBookmarksResponse;
using Serilog;

namespace PixivSync.Database.Entites;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class User
{
    public long Id { get; init; }
    public string? Cookie { get; set; }

    private static int DivideCeiling(int x, int y)
    {
        return (x + y - 1) / y;
    }

    public async Task<IllustBookmarkInfo[]> GetAllBookmarks(bool @private)
    {
        Log.Information("开始获取所有收藏");
        IPixivApi pixivApi = PixivApi.Default;

        string? cookie = @private ? Cookie : null;

        Task<GetBookmarksResponse> GetBookmarks(int offset)
        {
            return pixivApi.GetBookmarks(Id, offset, 100, PixivApi.BookmarkRestrictType(@private),
                cookie);
        }

        List<Task<GetBookmarksResponse>> getBookmarksTasks = new() { GetBookmarks(0) };
        GetBookmarksResponse resp1 = await getBookmarksTasks[0];
        int total = resp1.Body.Total;

        for (var i = 1; i < DivideCeiling(total, 100); i++)
        {
            getBookmarksTasks.Add(GetBookmarks(i * 100));
        }

        await Task.WhenAll(getBookmarksTasks);
        IllustBookmarkInfo[] result = getBookmarksTasks.SelectMany(t => t.Result.Body.Works).ToArray();
        Log.Information("获取到 {Count} 项", result.Length);
        return result;
    }

    public IAsyncEnumerable<IllustBookmarkInfo> GetAddedBookmarks(bool @private)
    {
        Log.Information("开始获取添加的收藏");
        using ISession session = Db.SessionFactory.OpenSession();
        HashSet<long> ids = session.Query<Illust>().Select(illust => illust.Id).ToHashSet();
        return EnumerateBookmarks(@private).TakeWhile(bookmarkInfo => !ids.Contains(Convert.ToInt64(bookmarkInfo.Id)));
    }

    public async IAsyncEnumerable<IllustBookmarkInfo> EnumerateBookmarks(bool @private)
    {
        Log.Information("开始枚举收藏");
        IPixivApi pixivApi = PixivApi.Default;

        int total;
        var offset = 0;
        do
        {
            GetBookmarksResponse resp =
                await pixivApi.GetBookmarks(Id, offset, 100, PixivApi.BookmarkRestrictType(@private), Cookie);
            total = resp.Body.Total;

            foreach (IllustBookmarkInfo bookmarkInfo in resp.Body.Works)
            {
                yield return bookmarkInfo;
            }

            offset += resp.Body.Works.Count;
        } while (offset < total);
    }
}