﻿using PixivSync.Pixiv;
using PixivSync.Pixiv.ApiResponse.GetBookmarksResponse;
using Spectre.Console.Cli;

namespace PixivSync;

public sealed class SyncPixivCommand : AsyncCommand<SyncPixivCommand.Settings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var config = Config.Default;

        var user = new User { Id = config.Auth.Id, Cookie = config.Auth.Cookie };

        IAsyncEnumerable<IllustBookmarkInfo> bookmarkInfos = settings.Merge
            ? (await user.GetAllBookmarks()).ToAsyncEnumerable()
            : user.GetAddedBookmarks();

        Illust[] illusts = await Illust.FromBookmarkInfo(bookmarkInfos).ToArrayAsync();

        if (settings.Merge)
            await Database.Merge(illusts.ToAsyncEnumerable());
        else
            await Database.Add(illusts.ToAsyncEnumerable());

        var storage = Storage.Default;
        storage.ResolveArtistNameChanges(); // Resolve before download to avoid duplicates

        await storage.BeginDownload(illusts.ToAsyncEnumerable());

        return 0;
    }

    public sealed class Settings : CommandSettings
    {
        [CommandOption("-m|--merge")] public bool Merge { get; init; }
    }
}