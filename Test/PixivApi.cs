using System.Text.Json;
using PixivSync.Pixiv.ApiResponse.GetBookmarksResponse;

namespace PixivSync.Test;

public class PixivApi
{
    [Theory]
    [InlineData(@"Data\GetBookmarksResponse\work.normal.json")]
    [InlineData(@"Data\GetBookmarksResponse\work.removed.json")]
    public void GetBookmarks_ParseWork(string jsonFilePath)
    {
        string json = File.ReadAllText(jsonFilePath);
        Exception? exception = Record.Exception(() => JsonSerializer.Deserialize<IllustBookmarkInfo>(json));
        Assert.Null(exception);
    }
}