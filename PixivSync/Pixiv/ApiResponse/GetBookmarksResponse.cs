// ReSharper disable All

#pragma warning disable

namespace PixivSync.Pixiv.ApiResponse.GetBookmarksResponse;

public sealed class Body
{
    public int total { get; set; }
    public List<IllustBookmarkInfo> works { get; set; }
}

public sealed class GetBookmarksResponse
{
    public Body body { get; set; }
    public bool error { get; set; }
    public string message { get; set; }
}

public sealed class IllustBookmarkInfo
{
    public object /* string or integer */ id { get; set; }

    // public string alt { get; set; }
    // public DateTime createDate { get; set; }
    // public string description { get; set; }
    // public int height { get; set; }
    // public int illustType { get; set; }
    // public bool isBookmarkable { get; set; }
    // public bool isMasked { get; set; }
    // public bool isUnlisted { get; set; }
    // public int pageCount { get; set; }
    // public string profileImageUrl { get; set; }
    // public int restrict { get; set; }
    // public int sl { get; set; }
    // public List<string> tags { get; set; }
    // public string title { get; set; }
    // public DateTime updateDate { get; set; }
    // public string url { get; set; }
    // public string userId { get; set; }
    // public string userName { get; set; }
    // public int width { get; set; }
    public int xRestrict { get; set; }
}