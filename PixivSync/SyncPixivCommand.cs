using NHibernate;
using PixivSync.Pixiv;
using PixivSync.Pixiv.ApiResponse.GetBookmarksResponse;
using Spectre.Console.Cli;

namespace PixivSync;

public sealed class SyncPixivCommand : AsyncCommand<SyncPixivCommand.Settings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var config = Config.Default;

        var user = new User { Id = config.Auth.Id, Cookie = config.Auth.Cookie };

        IAsyncEnumerable<IllustBookmarkInfo> bookmarkInfos = settings.Migrate
            ? (await user.GetAllBookmarks()).ToAsyncEnumerable()
            : user.GetAddedBookmarks();

        IAsyncEnumerable<Illust> illusts = Illust.FromBookmarkInfo(bookmarkInfos);
        await Database.Merge(illusts);

        var storage = Storage.Default;
        storage.ResolveArtistNameChanges(); // Resolve before download to avoid duplicates

        using (ISession session = Database.SessionFactory.OpenSession())
        {
            IQueryable<Illust> availableIllusts = session.Query<Illust>();
            await storage.BeginDownload(availableIllusts);
        }

        return 0;
    }

    public sealed class Settings : CommandSettings
    {
        [CommandOption("-m|--migrate")] public bool Migrate { get; init; }
    }
}