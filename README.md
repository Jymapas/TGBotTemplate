# Шаблон Telegram-бота (Clean Architecture)

Шаблон проекта на .NET 8 для создания Telegram-бота с использованием Clean Architecture. Проект разделён на слои Domain, Application, Infrastructure и Bot, использует EF Core с SQLite, Serilog для логирования и xUnit/FluentAssertions для тестов.

## Структура каталогов

```
├── src
│   ├── Domain                # Доменные сущности и константы
│   ├── Application           # Контракты и сообщения приложения
│   ├── Infrastructure        # EF Core, сервисы, миграции, настройки
│   └── Bot                   # Исполняемый проект бота и фоновая служба
├── tests                     # Модульные тесты
├── docker-compose.yml
├── Dockerfile
├── .env.example
└── .github/workflows/ci.yml
```

## Быстрый старт

1. Скопируйте файл `.env.example` в `.env` и заполните значения:
   ```bash
   cp .env.example .env
   ```
   Обязательные параметры:
   - `BOT_TOKEN` — токен, выданный BotFather.
   - `ADMIN_IDS` — Telegram userId администраторов через запятую.
   - `LOG_PATH`, `DB_PATH` — относительные пути для логов и базы данных.

2. Убедитесь, что установлены .NET SDK 8.0 и SQLite.

3. Восстановите зависимости и соберите решение:
   ```bash
   dotnet restore
   dotnet build
   ```

4. Запустите бота локально:
   ```bash
   dotnet run --project src/Bot
   ```

## База данных и миграции

Миграции EF Core находятся в `src/Infrastructure/Migrations`. Для их применения:
```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Bot
```

## Тестирование

```bash
dotnet test
```

Тесты покрывают `SessionService` и `RoleService`, используя xUnit и FluentAssertions.

## Запуск в Docker

```bash
docker compose build
docker compose up -d
```

Каталоги `logs/` и `data/` пробрасываются наружу для сохранения логов и базы данных. Переменные окружения подставляются из `.env`.

## CI/CD

В репозитории настроен GitHub Actions workflow `.github/workflows/ci.yml`, который выполняет восстановление, сборку и тестирование на каждый push и pull request в ветку `main`.

## Полезные команды

- Применение миграций: `dotnet ef database update`
- Создание новой миграции: `dotnet ef migrations add <Имя> --project src/Infrastructure --startup-project src/Bot`
- Локальный запуск: `dotnet run --project src/Bot`
- Просмотр логов: `tail -f logs/bot.log`

Проект разработан с учётом чистой архитектуры и готов к дальнейшему расширению бота под конкретные сценарии.
