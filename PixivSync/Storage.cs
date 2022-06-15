using System.IO;
using NHibernate;
using PixivSync.Pixiv;
using Serilog;

namespace PixivSync;

public class Storage
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
    public static Storage Default { get; } = new(Config.StorageRoot);

    public void ResolveArtistNameChanges()
    {
        Log.Information("正在处理画师改名情况");
        using ISession session = Database.SessionFactory.OpenSession();
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

    public async Task BeginSaveIllust(IEnumerable<Illust> illusts)
    {
        Log.Information("准备发送下载到 Aria2");
        var aria2 = new Aria2();
        foreach (Illust illust in illusts)
        {
            foreach (Page illustPage in illust.Pages!)
            {
                string saveDir = new DirectoryInfo(Path.Combine(Root.FullName,
                    $"{illust.Artist?.NormalizedName}_{illust.Artist?.Id ?? 0}")).FullName;
                string fileName = new Uri(illustPage.Original).Segments.Last();
                var fileInfo = new FileInfo(Path.Join(saveDir, fileName));

                if (fileInfo.Exists)
                {
                    Log.Verbose("跳过 PID {PID}：已有同名文件", illust.Id);
                    continue;
                }

                Log.Information("Aria2 添加 {PID}", illust.Id);

                await aria2.AddUri(illustPage.Original,
                    new AddUriParams
                    {
                        SaveDir = saveDir,
                        Referer = "https://www.pixiv.net",
                        Cookie = Config.AuxAccountCookie,
                        CondictionalGet = true,
                        AutoFileRenaming = true,
                        RetryWait = 5,
                        MaxConnection = 1
                    });
            }
        }

        Log.Information("已全部发送");
    }
}