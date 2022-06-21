using JetBrains.Annotations;
using Serilog.Events;
using YamlDotNet.Serialization;

namespace PixivSync;

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
public sealed class Config
{
    private const string FilePath = "config.yml";

    static Config()
    {
        if (!File.Exists(FilePath))
        {
            using var write = new StreamWriter(FilePath);
            new Serializer().Serialize(write, new Config());
            throw new Exception("未找到配置文件，已创建配置文件模板");
        }

        using StreamReader reader = File.OpenText(FilePath);
        Default = new Deserializer().Deserialize<Config>(reader);
    }

    public static Config Default { get; }

    public LogSettings Log { get; set; } = new();

    public AuthSettings Auth { get; set; } = new();

    public Aria2Settings Aria2 { get; set; } = new();

    public string StoragePath { get; set; } = string.Empty;
    public string DbPath { get; set; } = string.Empty;
    public bool UsePrivateBookmarks { get; set; }

    [UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class LogSettings
    {
        public string? Dir { get; set; }
        public LogEventLevel Level { get; set; } = LogEventLevel.Information;
    }

    [UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class AuthSettings
    {
        private string? _auxCookie;
        public long Id { get; set; } = 114514;
        public string Cookie { get; set; } = "PHPSESSID=xxxxxx";

        public string AuxCookie
        {
            get => _auxCookie ?? Cookie;
            set => _auxCookie = value;
        }
    }

    [UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class Aria2Settings
    {
        public string JsonRpcUrl { get; set; } = "http://127.0.0.1:6800";
        public string? RpcSecret { get; set; }
    }
}