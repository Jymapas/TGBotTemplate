using Bot.Handlers;
using Bot.HostedServices;
using Infrastructure.Configuration;
using Infrastructure.DependencyInjection;
using Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Telegram.Bot;

var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
var env = EnvLoader.Load(envFilePath);
EnvLoader.Apply(env);

var botOptions = new BotOptions
{
    Token = env.GetRequired("BOT_TOKEN"),
    LogPath = env.GetOptional("LOG_PATH", "logs/bot.log")!,
    DbPath = env.GetOptional("DB_PATH", "data/bot.db")!
};

EnsureDirectoryFor(botOptions.LogPath);
EnsureDirectoryFor(botOptions.DbPath);

var adminIds = ParseAdminIds(env.GetOptional("ADMIN_IDS", string.Empty));

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((_, _, loggerConfiguration) =>
    {
        loggerConfiguration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteTo.File(
                path: botOptions.LogPath,
                outputTemplate: "{Message:lj}{NewLine}",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true);
    })
    .ConfigureServices(services =>
    {
        services.AddInfrastructure(botOptions, adminIds);
        services.AddSingleton(sp => new TelegramBotClient(botOptions.Token));
        services.AddSingleton<BotUpdateHandler>();
        services.AddHostedService<BotHostedService>();
    })
    .Build();

await host.RunAsync();

static IReadOnlyCollection<long> ParseAdminIds(string? value)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        return Array.Empty<long>();
    }

    var result = new List<long>();

    foreach (var part in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    {
        if (long.TryParse(part, out var id))
        {
            result.Add(id);
        }
    }

    return result;
}

static void EnsureDirectoryFor(string path)
{
    var directory = Path.GetDirectoryName(path);

    if (string.IsNullOrWhiteSpace(directory))
    {
        return;
    }

    Directory.CreateDirectory(directory);
}
