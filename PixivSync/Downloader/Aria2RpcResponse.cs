using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace PixivSync.Downloader;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed record Aria2RpcResponse
{
    [JsonPropertyName("id")] public string? Id { get; init; }

    [JsonPropertyName("jsonrpc")] public string? JsonRpc { get; init; }

    [JsonPropertyName("error")] public Error? Error { get; init; }
    [JsonPropertyName("result")] public string? Result { get; init; }
}

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed record Error
{
    [JsonPropertyName("code")] public int Code { get; init; }

    [JsonPropertyName("message")] public string? Message { get; init; }
}