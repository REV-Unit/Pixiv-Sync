using System.Collections.Concurrent;
using PixivSync.Pixiv.ApiResponse.GetBookmarksResponse;
using Serilog;

namespace PixivSync.Pixiv;

public static class Account
{
    public static async Task<IllustBookmarkInfo[]> GetAllBookmarks(long id, string? cookie = null)
    {
        Log.Information("获取所有收藏中");
        IPixivApi pixivApi = PixivApi.Default;

        GetBookmarksResponse resp1 =
            await pixivApi.GetBookmarks(id, 0, 100, cookie);
        int total = resp1.body.total;
        var bag = new ConcurrentBag<List<IllustBookmarkInfo>> { resp1.body.works };
        await Parallel.ForEachAsync(Enumerable.Range(1, (int)Math.Ceiling(total / 100.0) + 1), async (i, _) =>
        {
            GetBookmarksResponse resp2 =
                await pixivApi.GetBookmarks(id, i * 100, 100, cookie);
            bag.Add(resp2.body.works);
        });
        IllustBookmarkInfo[] result = bag.SelectMany(l => l).ToArray();
        Log.Information("获取完毕");
        return result;
    }
}