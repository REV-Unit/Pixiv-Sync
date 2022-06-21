using NHibernate;
using PixivSync.Database;
using PixivSync.Database.Entites;
using Serilog;

namespace PixivSync;

public sealed class Storage
{
    public Storage(string storageRootPath)
    {
        Root = new DirectoryInfo(storageRootPath);
        if (!Root.Exists)
        {
            Root.Create();
        }
    }

    public DirectoryInfo Root { get; set; }
    public static Storage Default { get; } = new(Config.Default.StoragePath);

    public void ResolveArtistNameChanges()
    {
        Log.Information("正在处理画师改名情况");
        using ISession session = Db.SessionFactory.OpenSession();
        foreach (IGrouping<long, DirectoryInfo> group in Root.GetDirectories()
                     .GroupBy(d => long.Parse(d.Name.Split('_').Last())))
        {
            var artist = session.Get<Artist>(group.Key);
            if (artist == null) continue;

            var dest = new DirectoryInfo(
                Path.Combine(Root.FullName, $"{artist.NormalizedName}_{group.Key}"));
            if (!dest.Exists)
            {
                dest.Create();
            }

            foreach (DirectoryInfo duplicated in group.Where(d => d.FullName != dest.FullName))
            {
                Log.Information("{dup} -> {dest}", duplicated.Name, dest.Name);
                foreach (FileInfo picture in duplicated.GetFiles())
                {
                    string destPath = Path.Combine(dest.FullName, picture.Name);
                    if (File.Exists(destPath))
                    {
                        continue;
                    }

                    picture.MoveTo(destPath);
                }

                duplicated.Delete(true);
            }
        }
    }

    public async Task BeginDownload(IAsyncEnumerable<Illust> illusts)
    {
        Log.Information("开始发送下载到 Aria2");
        var aria2 = new Aria2(Config.Default.Aria2.JsonRpcUrl, Config.Default.Aria2.RpcSecret);
        int sentCount = 0, skippedCount = 0;

        await foreach (Illust illust in illusts.Where(illust => !illust.Deleted && illust.Type != IllustType.Ugoira))
        {
            foreach (Page illustPage in illust.Pages!)
            {
                string fileName = new Uri(illustPage.Original).Segments.Last();
                string pageName = Path.GetFileNameWithoutExtension(fileName);

                string saveDir = new DirectoryInfo(Path.Combine(Root.FullName,
                    $"{illust.Artist?.NormalizedName}_{illust.Artist?.Id ?? 0}")).FullName;
                var fileInfo = new FileInfo(Path.Join(saveDir, fileName));

                if (fileInfo.Exists)
                {
                    Log.Information("跳过 {PageName}：已有同名文件", pageName);
                    skippedCount++;
                    continue;
                }

                Log.Information("发送下载 {PageName}", pageName);

                await aria2.AddUri(illustPage.Original,
                    new AddUriOptions
                    {
                        SaveDir = saveDir,
                        Referer = "https://www.pixiv.net",
                        Cookie = Config.Default.Auth.AuxCookie,
                        CondictionalGet = true,
                        AutoFileRenaming = true,
                        RetryWait = 5,
                        MaxConnection = 1
                    });
                sentCount++;
            }
        }

        Log.Information("已发送下载 {Sent} 个，跳过 {Skip} 个", sentCount, skippedCount);
    }
}