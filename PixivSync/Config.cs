using System.IO;
using Microsoft.Extensions.Configuration;
using NetEscapades.Configuration.Yaml;
using Serilog.Events;
using YamlDotNet.Serialization;

namespace PixivSync;

public class Config
{
    public const string FilePath = "config.yml";

    private static readonly IConfiguration Configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddYamlFile(FilePath)
        //.AddCommandLine(args) https://github.com/dotnet/runtime/issues/36024
        .Build();

    static Config()
    {
        // using (StreamReader reader = File.OpenText(FilePath))
        // {
        //     new Deserializer().Deserialize<Config>(reader);
        // }
        
        Configuration.Bind(Default);
    }

    public static Config Default { get; } = new();

    public LogSettings Log { get; set; } = new();

    public AuthSettings Auth { get; set; } = new();

    public string StoragePath { get; set; }
    public string DbPath { get; set; }

    public class LogSettings
    {
        public string Path { get; set; }
        public LogEventLevel Level { get; set; }
    }

    public class AuthSettings
    {
        private string _auxCookie;
        public long Id { get; set; }
        public string Cookie { get; set; }

        public string AuxCookie
        {
            get => _auxCookie ?? Cookie;
            set => _auxCookie = value;
        }
    }
}