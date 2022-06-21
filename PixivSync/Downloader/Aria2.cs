using Refit;

namespace PixivSync.Downloader;

[Headers("Content-Type: application/json;charset=UTF-8")]
public interface IAria2Rpc
{
    [Post("/jsonrpc")]
    Task<Aria2RpcResponse> Invoke(JsonRpcParams rpcParams);
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