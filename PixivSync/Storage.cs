﻿using System.IO;
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
    public static Storage Default { get; } = new(Config.Default.StoragePath);

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

    public async Task BeginDownload(IAsyncEnumerable<Illust> illusts)
    {
        Log.Information("开始发送下载到 Aria2");
        var aria2 = new Aria2();
        int sentCount = 0, skippedCount = 0;

        await foreach (Illust illust in illusts.Where(illust => !illust.Deleted))
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
                    new AddUriParams
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

        if (sentCount != 0)
            Log.Information("已发送下载 {Sent} 个，跳过 {Skip} 个", sentCount, skippedCount);
        else
            Log.Information("未发送任何下载，跳过 {Skip} 个", skippedCount);
    }
}