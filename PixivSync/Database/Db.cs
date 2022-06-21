using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using PixivSync.Database.Entites;
using Serilog;

namespace PixivSync.Database;

public static class Db
{
    static Db()
    {
        ConfigurationProvider.Current = null; // https://github.com/nhibernate/fluent-nhibernate/discussions/531
        SessionFactory = Fluently.Configure()
            .Database(SQLiteConfiguration.Standard.UsingFile(Config.Default.DbPath))
            .Mappings(mappings =>
                mappings.FluentMappings
                    .AddFromAssembly(Assembly.GetExecutingAssembly())
                    .Conventions.Add(ForeignKey.EndsWith("Id"), DefaultLazy.Never()))
            .ExposeConfiguration(UpdateSchema)
            .BuildSessionFactory();
    }

    public static ISessionFactory SessionFactory { get; }

    private static void UpdateSchema(Configuration cfg)
    {
        Log.Information("更新数据库架构");
        new SchemaUpdate(cfg).Execute(true, true);
        Log.Information("更新完毕");
    }

    public static async Task Add(IAsyncEnumerable<Illust> illusts)
    {
        Log.Information("开始添加到数据库");
        using ISession session = SessionFactory.OpenSession();
        using ITransaction transaction = session.BeginTransaction();
        await foreach (Illust illust in illusts)
        {
            await session.SaveAsync(illust);
        }

        await transaction.CommitAsync();
        Log.Information("添加完毕");
    }

    public static async Task Merge(IAsyncEnumerable<Illust> illusts)
    {
        Log.Information("开始合并到数据库");
        using ISession session = SessionFactory.OpenSession();
        using ITransaction transaction = session.BeginTransaction();
        await foreach (Illust illust in illusts)
        {
            if (illust.Deleted && await session.GetAsync<Illust>(illust.Id) != null)
            {
                Log.Debug("PID {PID} 已被删除且数据库里已存在，跳过合并", illust.Id);
                continue;
            }

            await session.MergeAsync(illust);
        }

        await transaction.CommitAsync();
        Log.Information("合并完毕");
    }
}