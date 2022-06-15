using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using PixivSync.Pixiv;
using Serilog;

namespace PixivSync;

public static class Database
{
    public static ISessionFactory SessionFactory { get; } = Fluently.Configure()
        .Database(SQLiteConfiguration.Standard.UsingFile(Config.DbPath))
        .Mappings(mappings =>
            mappings.FluentMappings
                .AddFromAssembly(Assembly.GetExecutingAssembly())
                .Conventions.Add(ForeignKey.EndsWith("Id"), DefaultLazy.Never()))
        .ExposeConfiguration(UpdateSchema)
        .BuildSessionFactory();

    private static void UpdateSchema(Configuration cfg)
    {
        Log.Information("更新数据库架构");
        new SchemaUpdate(cfg).Execute(true, true);
        Log.Information("更新完毕");
    }

    public static async Task Merge(IEnumerable<Illust> illusts)
    {
        Log.Information("合并到数据库");
        using ISession session = SessionFactory.OpenSession();
        using ITransaction transaction = session.BeginTransaction();
        foreach (Illust illust in illusts)
        {
            var oldInfo = session.Get<Illust>(illust.Id);
            if (oldInfo != null && illust.Deleted)
            {
                continue;
            }

            session.Merge(illust);
        }

        await transaction.CommitAsync();
        Log.Information("合并完毕");
    }
}