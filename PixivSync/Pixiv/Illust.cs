using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using PixivSync.Pixiv.ApiResponse.GetBookmarksResponse;
using PixivSync.Pixiv.ApiResponse.GetIllustInfoResponse;
using PixivSync.Pixiv.ApiResponse.GetIllustPagesResponse;
using Polly;
using Refit;
using Serilog;

namespace PixivSync.Pixiv;

public class Illust
{
    public virtual long Id { get; init; }
    public virtual Artist? Artist { get; set; }
    public virtual string? Title { get; set; }
    public virtual string? Description { get; set; }
    public virtual ISet<Tag>? Tags { get; set; }
    public virtual RestrictType RestrictType { get; set; }
    public virtual DateTime CreateDate { get; set; }
    public virtual DateTime UploadDate { get; set; }
    public virtual IList<Page> Pages { get; set; }
    public virtual bool Deleted { get; set; }

    public static async Task<Illust[]> FromBookmarkInfo(IEnumerable<IllustBookmarkInfo> bookmarks)
    {
        Log.Information("从 Pixiv 获取详细信息中");
        var bag = new ConcurrentBag<Illust>();
        await Parallel.ForEachAsync(bookmarks, async (bookmark, _) => bag.Add(await FromBookmarkInfo(bookmark)));
        Illust[] result = bag.ToArray();
        Log.Information("获取完毕");
        return result;
    }

    public static async Task<Illust> FromBookmarkInfo(IllustBookmarkInfo bookmarkInfo)
    {
        IPixivApi pixivApi = PixivApi.Default;
        IllustInfo illustInfo;
        long illustId = long.Parse(bookmarkInfo.id.ToString()!);
        List<PageInfo> pages;
        try
        {
            string? cookie = bookmarkInfo.xRestrict == 1 ? Config.AuxAccountCookie : null;
            IAsyncPolicy retryPolicy = Policy.Handle<HttpRequestException>().RetryAsync(3);
            illustInfo = (await retryPolicy.ExecuteAsync(() =>
                pixivApi.GetIllustInfo(illustId, cookie))).body;
            pages = (await retryPolicy.ExecuteAsync(() =>
                pixivApi.GetIllustPages(illustId, cookie))).body;
        }
        catch (ApiException e)
        {
            if (e.StatusCode != HttpStatusCode.NotFound) throw;

            Log.Warning($"PID {illustId} 已被删除");
            return new Illust
            {
                Id = illustId,
                Deleted = true
            };
        }

        return ParseFromInfo(illustInfo, pages);
    }

    public static Illust ParseFromInfo(IllustInfo illustInfo, IEnumerable<PageInfo> pageInfos)
    {
        long illustId = long.Parse(illustInfo.id);
        return new Illust
        {
            Id = illustId,
            Artist = new Artist
            {
                Id = long.Parse(illustInfo.userId),
                Name = illustInfo.userName
            },
            Title = illustInfo.title,
            Description = illustInfo.description,
            RestrictType = illustInfo switch
            {
                { restrict: 1 } => RestrictType.Sensitive,
                { xRestrict: 1 } => RestrictType.R18,
                _ => RestrictType.None
            },
            CreateDate = illustInfo.createDate,
            UploadDate = illustInfo.uploadDate,
            Tags = illustInfo.tags.tags.Select(t => new Tag
            {
                Name = t.tag,
                Translation = t.translation?.en,
                RomajiName = t.romaji
            }).ToHashSet(),
            Pages = pageInfos.Select((pageInfo, i) => new Page
            {
                Id = new PageId { IllustId = illustId, Number = i },
                Width = pageInfo.width,
                Height = pageInfo.height,
                ThumbMini = pageInfo.urls.thumb_mini,
                Small = pageInfo.urls.small,
                Regular = pageInfo.urls.regular,
                Original = pageInfo.urls.original
            }).ToArray()
        };
    }
}

public enum RestrictType
{
    None,
    Sensitive,
    R18
}