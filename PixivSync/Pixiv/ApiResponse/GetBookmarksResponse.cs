using System.Text.Json.Serialization;

// ReSharper disable All
namespace PixivSync.Pixiv.ApiResponse.GetBookmarksResponse;

public record Body([property: JsonPropertyName("works")] IReadOnlyList<IllustBookmarkInfo> Works,
    [property: JsonPropertyName("total")] int Total);

public record BookmarkData([property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("private")] bool Private);

public record GetBookmarksResponse([property: JsonPropertyName("error")] bool Error,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("body")] Body Body);

public record TitleCaptionTranslation([property: JsonPropertyName("workTitle")] object WorkTitle,
    [property: JsonPropertyName("workCaption")] object WorkCaption);

public record IllustBookmarkInfo([property: JsonPropertyName("id"), JsonConverter(typeof(IdConverter))] long Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("illustType")] int IllustType,
    [property: JsonPropertyName("xRestrict")] int XRestrict,
    [property: JsonPropertyName("restrict")] int Restrict,
    [property: JsonPropertyName("sl")] int Sl,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("tags")] IReadOnlyList<string> Tags,
    [property: JsonPropertyName("userId"), JsonConverter(typeof(IdConverter))] long UserId,
    [property: JsonPropertyName("userName")] string UserName,
    [property: JsonPropertyName("width")] int Width,
    [property: JsonPropertyName("height")] int Height,
    [property: JsonPropertyName("pageCount")] int PageCount,
    [property: JsonPropertyName("isBookmarkable")] bool IsBookmarkable,
    [property: JsonPropertyName("bookmarkData")] BookmarkData BookmarkData,
    [property: JsonPropertyName("alt")] string Alt,
    [property: JsonPropertyName("titleCaptionTranslation")] TitleCaptionTranslation TitleCaptionTranslation,
    [property: JsonPropertyName("createDate")] DateTime CreateDate,
    [property: JsonPropertyName("updateDate")] DateTime UpdateDate,
    [property: JsonPropertyName("isUnlisted")] bool IsUnlisted,
    [property: JsonPropertyName("isMasked")] bool IsMasked,
    [property: JsonPropertyName("profileImageUrl")] string ProfileImageUrl);