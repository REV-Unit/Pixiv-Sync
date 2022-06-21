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
    private readonly string? _rpcSecret;

    public Aria2(string jsonRpcUrl, string? rpcSecret = null)
    {
        _rpcSecret = rpcSecret;
        _rpc = RestService.For<IAria2Rpc>(jsonRpcUrl, new RefitSettings { Buffered = true });
    }

    public Task<Aria2RpcResponse> AddUri(string uri, AddUriOptions addUriOptions)
    {
        var addUriParams = new List<object>();
        if (_rpcSecret != null)
        {
            addUriParams.Add($"token:{_rpcSecret}");
        }

        addUriParams.Add(new[] { uri });
        addUriParams.Add(addUriOptions);
        var @params = new JsonRpcParams("aria2.addUri", addUriParams.ToArray());
        return _rpc.Invoke(@params);
    }
}