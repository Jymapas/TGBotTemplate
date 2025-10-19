namespace Application.Messages;

public static class Messages
{
    public const string Start =
        "<b>Привет!</b>\nЯ помогу подготовить текстовые объявления. Используй /help, чтобы узнать команды.";

    public const string Help =
        "<b>Доступные команды</b>\n/start — запустить бота\n/help — показать эту подсказку\n/cancel — сбросить текущий шаг.";

    public const string Cancel =
        "<i>Текущее действие отменено.</i>\nМожешь начинать заново.";

    public const string Unknown =
        "<i>Не понимаю команду.</i> Используй /help, чтобы посмотреть доступный список.";
}
