using System.IO;
using Serilog.Events;
using YamlDotNet.Serialization;

namespace PixivSync;

public class Config
{
    private const string FilePath = "config.yml";

    static Config()
    {
        if (!File.Exists(FilePath))
        {
            using var write = new StreamWriter(FilePath);
            new Serializer().Serialize(write, new Config());
            throw new Exception("Config does not exist, created example config.");
        }

        using StreamReader reader = File.OpenText(FilePath);
        Default = new Deserializer().Deserialize<Config>(reader);
    }

    public static Config Default { get; }

    public LogSettings Log { get; set; } = new();

    public AuthSettings Auth { get; set; } = new();

    public string StoragePath { get; set; } = string.Empty;
    public string DbPath { get; set; } = string.Empty;

    public class LogSettings
    {
        public string? Path { get; set; }
        public LogEventLevel Level { get; set; } = LogEventLevel.Information;
    }

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
}