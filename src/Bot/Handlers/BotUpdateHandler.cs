using System.Text.Json;
using Application.Interfaces;
using Application.Messages;
using Domain.Constants;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;

namespace Bot.Handlers;

public class BotUpdateHandler
{
    private readonly TelegramBotClient _botClient;
    private readonly ISessionService _sessionService;
    private readonly IRoleService _roleService;
    private readonly ILogger<BotUpdateHandler> _logger;

    public BotUpdateHandler(
        TelegramBotClient botClient,
        ISessionService sessionService,
        IRoleService roleService,
        ILogger<BotUpdateHandler> logger)
    {
        _botClient = botClient;
        _sessionService = sessionService;
        _roleService = roleService;
        _logger = logger;
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.Message is { } message)
        {
            await HandleMessageAsync(message, cancellationToken);
        }
    }

    private async Task HandleMessageAsync(Message message, CancellationToken cancellationToken)
    {
        if (message.From?.Id is null)
        {
            return;
        }

        var userId = message.From.Id;
        var chatId = message.Chat.Id;
        var text = message.Text ?? string.Empty;

        _logger.LogInformation("Получено сообщение от {UserId}: {Text}", userId, text);

        if (text.StartsWith("/", StringComparison.Ordinal))
        {
            await HandleCommandAsync(chatId, userId, text, cancellationToken);
            return;
        }

        var session = await _sessionService.GetOrCreateAsync(userId, cancellationToken);

        var payload = JsonSerializer.Serialize(new
        {
            text,
            session.State
        });

        await _sessionService.SetStateAsync(userId, SessionStates.PostText, payload, cancellationToken);

        var roleSuffix = _roleService.IsAdmin(userId) ? "\n<i>Роль: администратор.</i>" : string.Empty;

        await _botClient.SendMessage(
            chatId: chatId,
            text: $"<b>Спасибо!</b> Текст записан в черновик.{roleSuffix}",
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    private async Task HandleCommandAsync(long chatId, long userId, string command, CancellationToken cancellationToken)
    {
        switch (command)
        {
            case "/start":
                await _sessionService.ResetAsync(userId, cancellationToken);
                await _botClient.SendMessage(chatId, Messages.Start, ParseMode.Html, cancellationToken: cancellationToken);
                break;

            case "/help":
                await _botClient.SendMessage(chatId, Messages.Help, ParseMode.Html, cancellationToken: cancellationToken);
                break;

            case "/cancel":
                await _sessionService.ResetAsync(userId, cancellationToken);
                await _botClient.SendMessage(chatId, Messages.Cancel, ParseMode.Html, cancellationToken: cancellationToken);
                break;

            default:
                await _botClient.SendMessage(chatId, Messages.Unknown, ParseMode.Html, cancellationToken: cancellationToken);
                break;
        }
    }
}
