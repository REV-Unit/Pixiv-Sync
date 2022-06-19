﻿using System.Net;
using JetBrains.Annotations;
using PixivSync.Pixiv.ApiResponse.GetBookmarksResponse;
using PixivSync.Pixiv.ApiResponse.GetIllustInfoResponse;
using PixivSync.Pixiv.ApiResponse.GetIllustPagesResponse;
using Polly;
using Refit;
using Serilog;

namespace PixivSync.Pixiv;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class Illust
{
    public long Id { get; init; }
    public virtual Artist? Artist { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public virtual ISet<Tag>? Tags { get; set; }
    public RestrictType RestrictType { get; set; }
    public IllustType Type { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UploadDate { get; set; }
    public virtual IList<Page>? Pages { get; set; }
    public bool Deleted { get; set; }

    public static async IAsyncEnumerable<TResult> ConcurrentSelectAsync<T, TResult>(IAsyncEnumerable<T> source,
        Func<T, Task<TResult>> selector, int maximumConcurrency)
    {
        using var semaphore = new SemaphoreSlim(maximumConcurrency);

        static async Task<TResult> Limit(Func<T, Task<TResult>> func, T param, SemaphoreSlim semaphore)
        {
            TResult result = await func(param);
            semaphore.Release();
            return result;
        }

        var tasks = new HashSet<Task<TResult>>();
        await foreach (T item in source)
        {
            await semaphore.WaitAsync();
            tasks.Add(Limit(selector, item, semaphore));
        }

        while (tasks.Any())
        {
            Task<TResult> task = await Task.WhenAny(tasks);
            yield return task.Result;
            tasks.Remove(task);
        }
    }

    public static IAsyncEnumerable<Illust> FromBookmarkInfo(IAsyncEnumerable<IllustBookmarkInfo> bookmarks)
    {
        Log.Information("开始获取插画详细信息");
        return ConcurrentSelectAsync(bookmarks, FromBookmarkInfo, 50);
    }

    public static async Task<Illust> FromBookmarkInfo(IllustBookmarkInfo bookmarkInfo)
    {
        IPixivApi pixivApi = PixivApi.Default;
        IllustInfo illustInfo;
        long illustId = long.Parse(bookmarkInfo.id.ToString()!);
        List<PageInfo> pages;
        try
        {
            string? cookie = bookmarkInfo.xRestrict == 1 ? Config.Default.Auth.AuxCookie : null;
            IAsyncPolicy retryPolicy = Policy.Handle<HttpRequestException>().RetryAsync(3);

            Task<GetIllustInfoResponse> illustInfoTask = retryPolicy.ExecuteAsync(() =>
                pixivApi.GetIllustInfo(illustId, cookie));
            Task<GetIllustPagesResponse> pagesTask = retryPolicy.ExecuteAsync(() =>
                pixivApi.GetIllustPages(illustId, cookie));

            await Task.WhenAll(illustInfoTask, pagesTask);
            illustInfo = illustInfoTask.Result.body;
            pages = pagesTask.Result.body;
        }
        catch (ApiException e)
        {
            if (e.StatusCode != HttpStatusCode.NotFound) throw;

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
            Type = (IllustType)illustInfo.illustType,
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

public enum IllustType
{
    Illustration,
    Manga,
    Ugoira
}