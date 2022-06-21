using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace PixivSync.Downloader;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed record AddUriOptions
{
    [JsonPropertyName("out")] public string? SaveName { get; init; }

    [JsonPropertyName("dir")] public string? SaveDir { get; set; }

    [JsonPropertyName("referer")] public string? Referer { get; set; }

    [JsonPropertyName("cookie")] public string? Cookie { get; set; }

    [JsonPropertyName("auto-file-renaming")] public bool AutoFileRenaming { get; set; }

    [JsonPropertyName("condictional-get")] public bool CondictionalGet { get; set; }

    [JsonPropertyName("retry-wait")] public int RetryWait { get; set; }

    [JsonPropertyName("max-connection-per-server")] public int MaxConnection { get; set; }
}