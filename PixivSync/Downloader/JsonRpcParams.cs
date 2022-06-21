using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace PixivSync.Downloader;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed record JsonRpcParams(string Method, [property: JsonPropertyName("params")] params object[] Params)
{
#pragma warning disable CA1822 // 将成员标记为 static
    [JsonPropertyName("jsonrpc")] public string JsonRpc => "2.0";

    [JsonPropertyName("id")] public string Id => "PixivSync";
#pragma warning restore CA1822 // 将成员标记为 static
}