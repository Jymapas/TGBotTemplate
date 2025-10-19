using Bot.Handlers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Bot.HostedServices;

public class BotHostedService : BackgroundService
{
    private readonly TelegramBotClient _botClient;
    private readonly ILogger<BotHostedService> _logger;
    private readonly BotUpdateHandler _updateHandler;

    public BotHostedService(
        TelegramBotClient botClient,
        ILogger<BotHostedService> logger,
        BotUpdateHandler updateHandler)
    {
        _botClient = botClient;
        _logger = logger;
        _updateHandler = updateHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConfigureCommandsAsync(stoppingToken);

        _logger.LogInformation("Бот запущен и ожидает обновления.");

        await PollUpdatesAsync(stoppingToken);
    }

    private async Task ConfigureCommandsAsync(CancellationToken cancellationToken)
    {
        await _botClient.SetMyCommands(
            commands: new[]
            {
                new Telegram.Bot.Types.BotCommand { Command = "start", Description = "Запустить бота" },
                new Telegram.Bot.Types.BotCommand { Command = "help", Description = "Показать подсказку" },
                new Telegram.Bot.Types.BotCommand { Command = "cancel", Description = "Отменить текущий шаг" }
            },
            cancellationToken: cancellationToken);
    }

    private async Task PollUpdatesAsync(CancellationToken cancellationToken)
    {
        var offset = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var updates = await _botClient.GetUpdates(
                    offset: offset,
                    limit: 20,
                    timeout: 30,
                    allowedUpdates: Array.Empty<UpdateType>(),
                    cancellationToken: cancellationToken);

                foreach (var update in updates)
                {
                    offset = update.Id + 1;
                    await _updateHandler.HandleAsync(update, cancellationToken);
                }
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Ошибка при обработке обновлений. Повтор через 5 секунд.");
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }
}
