using CaynayBot.Data;
using CaynayBot.Models;
using CaynayBot.Repositories;
using CaynayBot.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace CaynayBot;

public class TelegramBotUpdateHandler
{
    public static bool _startUp = false;
    private readonly ITelegramBotClient _botClient;
    private readonly TLogger _logger;
    private CancellationToken _stoppingToken;

   
    public TelegramBotUpdateHandler(string token, TLogger logger)
    {
        _botClient = new TelegramBotClient(token);
        _logger = logger;
    }

    public void Start()
    {
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { },
        };

        _botClient.StartReceiving(
            (botClient, update, cancellationToken1) => HandleUpdateAsync(botClient, update, cancellationToken1),
            HandleErrorAsync, receiverOptions, cancellationToken);

    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var chatId = update.Message?.Chat.Id ?? update.CallbackQuery?.Message?.Chat.Id;

        if (update.Message?.From == null && update.CallbackQuery?.From == null)
        {
            _logger.LogWarning("Получено сообщение без информации о пользователе");
            return;
        }

        var connectionString = "server=127.0.0.1;uid=root;pwd=asde1D#cEC;database=randommatch;";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        

        try
        {
            using var dbContext = new AppDbContext(optionsBuilder.Options);
            var userService = new UserService(new Repository<TUser>(dbContext));
            var productService = new ProductService(new Repository<Product>(dbContext));
            var reportService = new ReportService(new Repository<Report>(dbContext));

            var from = update.Message?.From ?? update.CallbackQuery?.From;
            var message = update.Message?.Text ?? "";
            var data = update.CallbackQuery?.Data;

            // Проверяем подключение к БД перед работой
            if (!await IsDatabaseAvailable(dbContext, cancellationToken))
            {
                await SendErrorMessage(botClient,
                    "Сервис временно недоступен. Пожалуйста, попробуйте позже.");
                return;
            }

            // Получаем или создаем пользователя в отдельном подключении
            var user = await GetUserWithRetry(userService, from.Id, from.Username, from.FirstName, from.LastName, cancellationToken);

            if (user == null)
            {
                await SendErrorMessage(botClient,
                    "Произошла ошибка при загрузке данных пользователя.");
                return;
            }

            // Обрабатываем callback query
            if (data != null)
            {
                await Dialog.CallMessage(
                    botClient,
                    userService,
                    productService,
                    reportService,
                    user,
                    data,
                    update.Message?.Photo,
                    update.CallbackQuery?.Message?.Id);
            }
            // Обрабатываем текстовое сообщение
            else if (user != null)
            {
                Console.WriteLine(message);
                await Dialog.TextMessage(
                    botClient,
                    dbContext,
                    userService,
                    productService,
                    reportService,
                    user,
                    message,
                    update.Message?.Photo);
            }

            // Сохраняем изменения пользователя с retry
            await UpdateUserWithRetry(userService, user, cancellationToken);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Ошибка базы данных при обработке сообщения от {UserId}");
            await SendErrorMessage(botClient,
                "⚠️ Ошибка базы данных. Пожалуйста, повторите действие позже.");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Операция отменена при обработке сообщения");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке сообщения от Telegram для пользователя {UserId}");

            // Не отправляем сообщение об ошибке, если это callback query (может вызвать повторные попытки)
            if (update.Message != null)
            {
                await SendErrorMessage(botClient,
                    "❌ Произошла ошибка. Пожалуйста, попробуйте еще раз.");
            }
        }
    }

    private async Task<bool> IsDatabaseAvailable(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        try
        {
            // Проверяем подключение к БД с таймаутом
            var timeoutTask = Task.Delay(5000, cancellationToken);
            var canConnectTask = dbContext.Database.CanConnectAsync(cancellationToken);

            var completedTask = await Task.WhenAny(canConnectTask, timeoutTask);

            if (completedTask == canConnectTask && await canConnectTask)
            {
                return true;
            }

            _logger.LogWarning("База данных недоступна или не отвечает");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при проверке подключения к БД");
            return false;
        }
    }

    private async Task<TUser?> GetUserWithRetry(IUserService userService, long telegramId, string? username,
        string? firstName, string? lastName, CancellationToken cancellationToken, int maxRetries = 3)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                return await userService.GetOrCreateUserAsync(telegramId, username, firstName, lastName);
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                _logger.LogWarning(ex, "Попытка {Attempt} получения пользователя не удалась. Повтор через 1 секунду...", i + 1);
                await Task.Delay(1000, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Все попытки получения пользователя не удались");
                throw;
            }
        }

        return null;
    }

    private async Task UpdateUserWithRetry(IUserService userService, TUser user, CancellationToken cancellationToken, int maxRetries = 3)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                await userService.UpdateUser(user);
                return;
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                _logger.LogWarning(ex, "Попытка {Attempt} обновления пользователя не удалась. Повтор через 1 секунду...", i + 1);
                await Task.Delay(1000, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Все попытки обновления пользователя не удались");
                throw;
            }
        }
    }

    private async Task SendErrorMessage(ITelegramBotClient botClient, string message, long? chatId = 958529372)
    {
        if (chatId.HasValue)
        {
            try
            {
                await botClient.SendMessage(
                    chatId.Value,
                    message,
                    cancellationToken: _stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось отправить сообщение об ошибке пользователю {ChatId}", chatId);
            }
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiEx => $"Ошибка Telegram API:\n{apiEx.ErrorCode}\n{apiEx.Message}",
            OperationCanceledException => "Операция отменена",
            _ => exception.ToString()
        };

        // Не логируем ошибки отмены как критические
        if (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Ошибка при получении обновлений");
        }

        return Task.CompletedTask;
    }
}