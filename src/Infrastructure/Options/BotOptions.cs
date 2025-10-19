namespace Infrastructure.Options;

public class BotOptions
{
    public string Token { get; set; } = string.Empty;

    public string LogPath { get; set; } = "logs/bot.log";

    public string DbPath { get; set; } = "data/bot.db";
}
