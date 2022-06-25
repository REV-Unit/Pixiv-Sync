using System.Text.Json.Serialization;

// ReSharper disable All

namespace PixivSync.Pixiv.ApiResponse.GetIllustInfoResponse;

public record GetIllustInfoResponse(
    [property: JsonPropertyName("error")] bool Error,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("body")] IllustInfo Body
);

public record IllustInfo(
    [property: JsonPropertyName("illustId"), JsonConverter(typeof(IdConverter))] long IllustId,
    [property: JsonPropertyName("illustTitle")] string IllustTitle,
    [property: JsonPropertyName("illustComment")] string IllustComment,
    [property: JsonPropertyName("id"), JsonConverter(typeof(IdConverter))] long Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("illustType")] int IllustType,
    [property: JsonPropertyName("createDate")] DateTime CreateDate,
    [property: JsonPropertyName("uploadDate")] DateTime UploadDate,
    [property: JsonPropertyName("restrict")] int Restrict,
    [property: JsonPropertyName("xRestrict")] int XRestrict,
    [property: JsonPropertyName("sl")] int Sl,
    [property: JsonPropertyName("urls")] Urls Urls,
    [property: JsonPropertyName("tags")] TagsInfo TagsInfo,
    [property: JsonPropertyName("alt")] string Alt,
    [property: JsonPropertyName("storableTags")] IReadOnlyList<string> StorableTags,
    [property: JsonPropertyName("userId"), JsonConverter(typeof(IdConverter))] long UserId,
    [property: JsonPropertyName("userName")] string UserName,
    [property: JsonPropertyName("userAccount")] string UserAccount,
    [property: JsonPropertyName("likeData")] bool LikeData,
    [property: JsonPropertyName("width")] int Width,
    [property: JsonPropertyName("height")] int Height,
    [property: JsonPropertyName("pageCount")] int PageCount,
    [property: JsonPropertyName("bookmarkCount")] int BookmarkCount,
    [property: JsonPropertyName("likeCount")] int LikeCount,
    [property: JsonPropertyName("commentCount")] int CommentCount,
    [property: JsonPropertyName("responseCount")] int ResponseCount,
    [property: JsonPropertyName("viewCount")] int ViewCount,
    // [property: JsonPropertyName("bookStyle")] int BookStyle,
    [property: JsonPropertyName("isHowto")] bool IsHowto,
    [property: JsonPropertyName("isOriginal")] bool IsOriginal,
    [property: JsonPropertyName("imageResponseOutData")] IReadOnlyList<object> ImageResponseOutData,
    [property: JsonPropertyName("imageResponseData")] IReadOnlyList<object> ImageResponseData,
    [property: JsonPropertyName("imageResponseCount")] int ImageResponseCount,
    [property: JsonPropertyName("pollData")] object PollData,
    [property: JsonPropertyName("seriesNavData")] object SeriesNavData,
    [property: JsonPropertyName("descriptionBoothId")] object DescriptionBoothId,
    [property: JsonPropertyName("descriptionYoutubeId")] object DescriptionYoutubeId,
    [property: JsonPropertyName("comicPromotion")] object ComicPromotion,
    [property: JsonPropertyName("fanboxPromotion")] FanboxPromotion FanboxPromotion,
    [property: JsonPropertyName("contestBanners")] IReadOnlyList<object> ContestBanners,
    [property: JsonPropertyName("isBookmarkable")] bool IsBookmarkable,
    [property: JsonPropertyName("bookmarkData")] BookmarkData BookmarkData,
    [property: JsonPropertyName("contestData")] object ContestData,
    [property: JsonPropertyName("titleCaptionTranslation")] TitleCaptionTranslation TitleCaptionTranslation,
    [property: JsonPropertyName("isUnlisted")] bool IsUnlisted,
    [property: JsonPropertyName("request")] object Request,
    [property: JsonPropertyName("commentOff")] int CommentOff
);

public record TagsInfo(
    [property: JsonPropertyName("authorId")] string AuthorId,
    [property: JsonPropertyName("isLocked")] bool IsLocked,
    [property: JsonPropertyName("tags")] IReadOnlyList<Tag> Tags,
    [property: JsonPropertyName("writable")] bool Writable
);

public record BookmarkData(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("private")] bool Private
);

public record FanboxPromotion(
    [property: JsonPropertyName("userName")] string UserName,
    [property: JsonPropertyName("userImageUrl")] string UserImageUrl,
    [property: JsonPropertyName("contentUrl")] string ContentUrl,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("imageUrl")] string ImageUrl,
    [property: JsonPropertyName("imageUrlMobile")] string ImageUrlMobile,
    [property: JsonPropertyName("hasAdultContent")] bool HasAdultContent
);

public record Tag(
    [property: JsonPropertyName("tag")] string Name,
    [property: JsonPropertyName("locked")] bool Locked,
    [property: JsonPropertyName("deletable")] bool Deletable,
    [property: JsonPropertyName("userId")] string UserId,
    [property: JsonPropertyName("romaji")] string Romaji,
    [property: JsonPropertyName("translation")] Translation? Translation,
    [property: JsonPropertyName("userName")] string UserName,
    [property: JsonPropertyName("authorId")] string AuthorId,
    [property: JsonPropertyName("isLocked")] bool IsLocked,
    [property: JsonPropertyName("tags")] IReadOnlyList<Tag> Tags,
    [property: JsonPropertyName("writable")] bool Writable
);

public record TitleCaptionTranslation(
    [property: JsonPropertyName("workTitle")] object WorkTitle,
    [property: JsonPropertyName("workCaption")] object WorkCaption
);

public record Translation(
    [property: JsonPropertyName("en")] string En
);

public record Urls(
    [property: JsonPropertyName("mini")] string Mini,
    [property: JsonPropertyName("thumb")] string Thumb,
    [property: JsonPropertyName("small")] string Small,
    [property: JsonPropertyName("regular")] string Regular,
    [property: JsonPropertyName("original")] string Original
);