using NHibernate;
using PixivSync.Pixiv;
using PixivSync.Pixiv.ApiResponse.GetBookmarksResponse;
using Serilog;
using Spectre.Console.Cli;

namespace PixivSync;

public sealed class SyncPixivCommand : AsyncCommand<SyncPixivCommand.Settings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var config = Config.Default;

        var user = new User { Id = config.Auth.Id, Cookie = config.Auth.Cookie };

        IllustBookmarkInfo[] bookmarkInfos = settings.Migrate
            ? await user.GetAllBookmarks()
            : await user.GetAddedBookmarks();

        if (bookmarkInfos.Length == 0)
        {
            Log.Information("不进行操作");
            return 1;
        }

        Illust[] illusts = await Illust.FromBookmarkInfo(bookmarkInfos);
        await Database.Merge(illusts);

        var storage = Storage.Default;
        storage.ResolveArtistNameChanges();

        using (ISession session = Database.SessionFactory.OpenSession())
        {
            IQueryable<Illust> availableIllusts = session.Query<Illust>();
            await storage.BeginSaveIllust(availableIllusts);
        }

        return 0;
    }

    public sealed class Settings : CommandSettings
    {
        [CommandOption("-m|--migrate")] public bool Migrate { get; init; }
    }
}