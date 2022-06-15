using Microsoft.Extensions.Configuration;

namespace PixivSync;

public static class Config
{
    public const string FilePath = "config.yml";

    public static IConfiguration Default { get; } = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
        .AddYamlFile(FilePath)
        //.AddCommandLine(args) https://github.com/dotnet/runtime/issues/36024
        .Build();

    public static string MainAccountCookie => Default["Auth:Cookie"];
    public static long MainAccountId => long.Parse(Default["Auth:Id"]);
    public static string AuxAccountCookie => Default["Download:Cookie"] ?? MainAccountCookie;
    public static string DbPath => Default["Database:Path"];
    public static string StorageRoot => Default["Download:Root"];
}