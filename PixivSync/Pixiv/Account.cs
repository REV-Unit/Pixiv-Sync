using PixivSync.Pixiv.ApiResponse.GetBookmarksResponse;
using Serilog;

namespace PixivSync.Pixiv;

public static class Account
{
    private static int DivideCeiling(int x, int y)
    {
        return (x + y - 1) / y;
    }

    public static async Task<IllustBookmarkInfo[]> GetAllBookmarks(long id, string? cookie = null)
    {
        Log.Information("获取所有收藏中");
        IPixivApi pixivApi = PixivApi.Default;

        List<Task<GetBookmarksResponse>> getBookmarksTasks = new() { pixivApi.GetBookmarks(id, 0, 100, cookie) };
        GetBookmarksResponse resp1 = await getBookmarksTasks[0];
        int total = resp1.body.total;

        for (var i = 1; i < DivideCeiling(total, 100); i++)
        {
            getBookmarksTasks.Add(pixivApi.GetBookmarks(id, i * 100, 100, cookie));
        }

        await Task.WhenAll(getBookmarksTasks);
        IllustBookmarkInfo[] result = getBookmarksTasks.SelectMany(t => t.Result.body.works).ToArray();
        Log.Information("获取到 {Count} 项", result.Length);
        return result;
    }

    public static async IAsyncEnumerable<IllustBookmarkInfo> EnumerateBookmarks(long id, string? cookie = null)
    {
        Log.Information("枚举收藏中");
        IPixivApi pixivApi = PixivApi.Default;

        int total;
        var offset = 0;
        do
        {
            GetBookmarksResponse resp =
                await pixivApi.GetBookmarks(id, 0, 100, cookie);
            total = resp.body.total;

            foreach (IllustBookmarkInfo bookmarkInfo in resp.body.works)
            {
                yield return bookmarkInfo;
            }

            offset += resp.body.works.Count;
        } while (offset < total);
    }
}