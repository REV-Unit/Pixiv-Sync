using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Refit;

namespace PixivSync;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed record JsonRpcParams(string Method, [property: JsonPropertyName("params")] params object[] Params)
{
#pragma warning disable CA1822 // 将成员标记为 static
    [JsonPropertyName("jsonrpc")] public string JsonRpc => "2.0";

    [JsonPropertyName("id")] public string Id => "PixivSync";
#pragma warning restore CA1822 // 将成员标记为 static
}

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed record AddUriParams
{
    [JsonPropertyName("out")] public string? SaveName { get; init; }

    [JsonPropertyName("dir")] public string? SaveDir { get; set; }

    [JsonPropertyName("referer")] public string? Referer { get; set; }

    [JsonPropertyName("cookie")] public string? Cookie { get; set; }

    [JsonPropertyName("auto-file-renaming")]
    public bool AutoFileRenaming { get; set; }

    [JsonPropertyName("condictional-get")] public bool CondictionalGet { get; set; }

    [JsonPropertyName("retry-wait")] public int RetryWait { get; set; }

    [JsonPropertyName("max-connection-per-server")]
    public int MaxConnection { get; set; }
}

[Headers("Content-Type: application/json;charset=UTF-8")]
public interface IAria2Rpc
{
    [Post("/jsonrpc")]
    Task<Aria2RpcResponse> Invoke(JsonRpcParams rpcParams);
}

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed record Error
{
    [JsonPropertyName("code")] public int Code { get; init; }

    [JsonPropertyName("message")] public string? Message { get; init; }
}

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed record Aria2RpcResponse
{
    [JsonPropertyName("id")] public string? Id { get; init; }

    [JsonPropertyName("jsonrpc")] public string? JsonRpc { get; init; }

    [JsonPropertyName("error")] public Error? Error { get; init; }
    [JsonPropertyName("result")] public string? Result { get; init; }
}

public sealed class Aria2
{
    private readonly IAria2Rpc _rpc;

    public Aria2(string hostUrl = "http://127.0.0.1:6800")
    {
        _rpc = RestService.For<IAria2Rpc>(hostUrl, new RefitSettings { Buffered = true });
    }

    public Task<Aria2RpcResponse> AddUri(string uri, AddUriParams addUriParams)
    {
        var @params = new JsonRpcParams("aria2.addUri", new[] { uri }, addUriParams);
        return _rpc.Invoke(@params);
    }
}