// ReSharper disable All

using System.Text.Json.Serialization;

namespace PixivSync.Pixiv.ApiResponse.GetIllustPagesResponse;

public record PageInfo(
    [property: JsonPropertyName("urls")] Urls Urls,
    [property: JsonPropertyName("width")] int Width,
    [property: JsonPropertyName("height")] int Height
);

public record GetIllustPagesResponse(
    [property: JsonPropertyName("error")] bool Error,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("body")] IReadOnlyList<PageInfo> Body
);

public record Urls(
    [property: JsonPropertyName("thumb_mini")] string ThumbMini,
    [property: JsonPropertyName("small")] string Small,
    [property: JsonPropertyName("regular")] string Regular,
    [property: JsonPropertyName("original")] string Original
);