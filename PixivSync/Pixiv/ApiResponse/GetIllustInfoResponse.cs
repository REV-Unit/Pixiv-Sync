// ReSharper disable All

#pragma warning disable

namespace PixivSync.Pixiv.ApiResponse.GetIllustInfoResponse;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class AlternateLanguages
{
    public string en { get; set; }
    public string ja { get; set; }
}

public class IllustInfo
{
    public string alt { get; set; }
    public int bookmarkCount { get; set; }
    public BookmarkData bookmarkData { get; set; }
    public object comicPromotion { get; set; }
    public int commentCount { get; set; }
    public int commentOff { get; set; }
    public List<object> contestBanners { get; set; }
    public object contestData { get; set; }
    public DateTime createDate { get; set; }
    public string description { get; set; }
    public object descriptionBoothId { get; set; }

    public string descriptionYoutubeId { get; set; }

    //public ExtraData extraData { get; set; }
    public object fanboxPromotion { get; set; }
    public int height { get; set; }
    public string id { get; set; }
    public string illustComment { get; set; }
    public string illustId { get; set; }
    public string illustTitle { get; set; }
    public int illustType { get; set; }
    public int imageResponseCount { get; set; }
    public List<object> imageResponseData { get; set; }
    public List<object> imageResponseOutData { get; set; }
    public bool isBookmarkable { get; set; }
    public bool isHowto { get; set; }
    public bool isOriginal { get; set; }
    public bool isUnlisted { get; set; }
    public int likeCount { get; set; }
    public bool likeData { get; set; }
    public int pageCount { get; set; }
    public object pollData { get; set; }
    public object request { get; set; }
    public int responseCount { get; set; }
    public int restrict { get; set; }
    public object seriesNavData { get; set; }
    public int sl { get; set; }
    public TagList tags { get; set; }
    public string title { get; set; }
    public TitleCaptionTranslation titleCaptionTranslation { get; set; }
    public DateTime uploadDate { get; set; }
    public Urls urls { get; set; }
    public string userAccount { get; set; }
    public string userId { get; set; }
    public string userName { get; set; }
    public int viewCount { get; set; }
    public int width { get; set; }
    public int xRestrict { get; set; }
}

public class BookmarkData
{
    public string id { get; set; }
    public bool @private { get; set; }
}

public class ExtraData
{
    public Meta meta { get; set; }
}

public class Meta
{
    public AlternateLanguages alternateLanguages { get; set; }
    public string canonical { get; set; }
    public string description { get; set; }
    public string descriptionHeader { get; set; }
    public Ogp ogp { get; set; }
    public string title { get; set; }
    public Twitter twitter { get; set; }
}

public class Ogp
{
    public string description { get; set; }
    public string image { get; set; }
    public string title { get; set; }
    public string type { get; set; }
}

public class GetIllustInfoResponse
{
    public IllustInfo body { get; set; }
    public bool error { get; set; }
    public string message { get; set; }
}

public class TagList
{
    public string authorId { get; set; }
    public bool isLocked { get; set; }
    public List<Tag> tags { get; set; }
    public bool writable { get; set; }
}

public class Tag
{
    public bool deletable { get; set; }
    public bool locked { get; set; }
    public string? romaji { get; set; }
    public string tag { get; set; }
    public Translation? translation { get; set; }
    public string userId { get; set; }
    public string userName { get; set; }
}

public class TitleCaptionTranslation
{
    public object workCaption { get; set; }
    public object workTitle { get; set; }
}

public class Translation
{
    public string en { get; set; }
}

public class Twitter
{
    public string card { get; set; }
    public string description { get; set; }
    public string image { get; set; }
    public string title { get; set; }
}

public class Urls
{
    public string mini { get; set; }
    public string original { get; set; }
    public string regular { get; set; }
    public string small { get; set; }
    public string thumb { get; set; }
}