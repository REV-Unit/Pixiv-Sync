using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using PixivSync.Pixiv;
using PixivSync.Pixiv.ApiResponse.GetBookmarksResponse;
using Serilog;

namespace PixivSync;

public static class Database
{
    public static ISessionFactory SessionFactory { get; } = Fluently.Configure()
        .Database(SQLiteConfiguration.Standard.UsingFile(Config.DbPath))
        .Mappings(mappings => mappings.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()).Conventions
            .Setup(c => c.Add(ForeignKey.EndsWith("Id"))))
        //.Mappings(mappings => mappings.AutoMappings.Add(AutoMap.Assembly(Assembly.GetExecutingAssembly())))
        .ExposeConfiguration(cfg =>
        {
            Log.Information("更新数据库架构");
            new SchemaUpdate(cfg).Execute(true, true);
            Log.Information("更新完毕");
        })
        .BuildSessionFactory();

    public static async Task<IllustBookmarkInfo[]> GetBookmarksToProcess(bool addedOnly)
    {
        if (!addedOnly)
        {
            return await Account.GetAllBookmarks(Config.MainAccountId, Config.MainAccountCookie);
        }

        using ISession session = SessionFactory.OpenSession();
        HashSet<long> ids = session.Query<Illust>().Select(illust => illust.Id).ToHashSet();
        IllustBookmarkInfo[] delta = await Account.EnumerateBookmarks(Config.MainAccountId, Config.MainAccountCookie)
            .TakeWhile(bookmarkInfo => ids.Contains(Convert.ToInt64(bookmarkInfo.id)))
            .ToArrayAsync();
        Log.Information("Delta {Delta} 个插画", delta.Length);
        return delta;
    }

    public static Illust[] Merge(IEnumerable<Illust> illusts)
    {
        Log.Information("合并到数据库");
        using ISession session = SessionFactory.OpenSession();
        using ITransaction transaction = session.BeginTransaction();
        Illust[] mergedInfo = illusts.Select(newInfo =>
        {
            var oldInfo = session.Get<Illust>(newInfo.Id);
            if (oldInfo != null && newInfo.Deleted)
            {
                return oldInfo;
            }

            return session.Merge(newInfo);
        }).ToArray();

        transaction.Commit();
        Log.Information("合并完毕");
        return mergedInfo;
    }
}