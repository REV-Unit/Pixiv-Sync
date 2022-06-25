using PixivSync.Pixiv.ApiResponse.GetBookmarksResponse;
using PixivSync.Pixiv.ApiResponse.GetIllustInfoResponse;
using PixivSync.Pixiv.ApiResponse.GetIllustPagesResponse;
using Refit;

namespace PixivSync.Pixiv;

[Headers("Referer: https://www.pixiv.net",
    "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36",
    "Accept-Language: zh-CN,zh;q=0.9,en;q=0.8,ja;q=0.7,zh-TW;q=0.6")]
public interface IPixivApi
{
    [Get("/ajax/user/{uid}/illusts/bookmarks?tag=")]
    Task<GetBookmarksResponse> GetBookmarks(long uid,
        [Query] int offset,
        [Query] int limit,
        [AliasAs("rest")] [Query] string restrictType,
        [Header("Cookie")] string? cookie = null
    );

    [Get("/ajax/illust/{pid}")]
    Task<GetIllustInfoResponse> GetIllustInfo(long pid, [Header("Cookie")] string? cookie = null);

    [Get("/ajax/illust/{pid}/pages")]
    Task<GetIllustPagesResponse> GetIllustPages(long pid, [Header("Cookie")] string? cookie = null);
}

public static class PixivApi
{
    public static IPixivApi Default { get; } = RestService.For<IPixivApi>("https://www.pixiv.net");

    public static string BookmarkRestrictType(bool @private)
    {
        return @private ? "hide" : "show";
    }
}