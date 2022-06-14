using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using PixivSync.Pixiv;
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