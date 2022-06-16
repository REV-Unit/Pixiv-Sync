using PixivSync.Pixiv;
using PixivSync.Pixiv.ApiResponse.GetBookmarksResponse;
using Serilog;
using Spectre.Console.Cli;

namespace PixivSync;

public sealed class SyncPixivCommand : AsyncCommand<SyncPixivCommand.Settings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        Log.Information("开始同步");
        var config = Config.Default;

        var user = new User { Id = config.Auth.Id, Cookie = config.Auth.Cookie };

        IAsyncEnumerable<IllustBookmarkInfo> bookmarkInfos = settings.Merge
            ? (await user.GetAllBookmarks(config.UsePrivateBookmarks)).ToAsyncEnumerable()
            : user.GetAddedBookmarks(config.UsePrivateBookmarks);

        Illust[] illusts = await Illust.FromBookmarkInfo(bookmarkInfos).ToArrayAsync();

        await Database.Merge(illusts.ToAsyncEnumerable());

        var storage = Storage.Default;
        storage.ResolveArtistNameChanges(); // Resolve before download to avoid duplicates

        await storage.BeginDownload(illusts.ToAsyncEnumerable());

        Log.Information("已完成同步");
        return 0;
    }

    public sealed class Settings : CommandSettings
    {
        [CommandOption("-m|--merge")] public bool Merge { get; init; }
    }
}