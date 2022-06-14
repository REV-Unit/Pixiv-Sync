// ReSharper disable All

#pragma warning disable

namespace PixivSync.Pixiv.ApiResponse.GetIllustPagesResponse;

public class GetIllustPagesResponse
{
    public bool error { get; set; }
    public string message { get; set; }
    public List<PageInfo> body { get; set; }
}

public class PageInfo
{
    public Urls urls { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

public class Urls
{
    public string thumb_mini { get; set; }
    public string small { get; set; }
    public string regular { get; set; }
    public string original { get; set; }
}