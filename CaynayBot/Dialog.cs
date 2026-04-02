using RandomMatch.Server.Data;
using RandomMatch.Server.Models;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RandomMatch.Server.Services;

internal class Dialog
{

    private static ReplyKeyboardMarkup keyboarRoles = new ReplyKeyboardMarkup(new KeyboardButton("Менеджер"), new KeyboardButton("Владелец")) { ResizeKeyboard = true };
    private static ReplyKeyboardMarkup mainKeyboardManager = new ReplyKeyboardMarkup(new KeyboardButton("Продажа"), new KeyboardButton("Закончить смену")) { ResizeKeyboard = true };
    private static ReplyKeyboardMarkup mainKeyboardAdmin = new ReplyKeyboardMarkup(new KeyboardButton("Отчёт"), new KeyboardButton("Работники"), new KeyboardButton("Панель")) { ResizeKeyboard = true};
    private static ReplyKeyboardMarkup mainKeyboardStart = new ReplyKeyboardMarkup(new KeyboardButton("Начать смену")) { ResizeKeyboard = true};
    

    public static async Task MessageToAdmins(ITelegramBotClient bot, IUserService userService,string message)
    {
        var users = await userService.GetAllAsync();
        users.ToList().ForEach(async user => 
        {
            if (user.Role == ChaynayRole.Admin) await bot.SendMessage(user.ChatId, message);
        });
    }
    public static async Task SaveReport(TUser user, IEnumerable<TUser> users, ITelegramBotClient bot, IReportService reportService, Report report, Product product, DateTime createdAt)
    {
        try
        {
            await reportService.AddReport(report);
            user.State = StateUser.Registred;
            users.ToList().ForEach(x =>
            {
                if (x.Role == ChaynayRole.Admin) bot.SendMessage(x.ChatId, $"Менеджер @{user.Username} добавил в отчёт:\n{product.Name} {report.Count} (гр.\\шт.)\nСумма: {(report.Count * product.Price):0.00} руб.");
            });
            var markup = new InlineKeyboardButton("Отмена", $"otmena_{createdAt.ToBinary()}");
            await bot.SendMessage(user.ChatId, $"Позиция {product.Name} добавлена в отчет!", replyMarkup: markup);

        }
        catch (Exception)
        {

        }
    }

    public static async Task TextMessage(ITelegramBotClient bot, AppDbContext dbContext, IUserService userService, IProductService productService, IReportService reportService, TUser user, string message, PhotoSize[]? photo = null, int messageId = 0)
    {
        try
        {
            var chatId = user.ChatId;
            switch (user.State)
            {
                #region Регистрация
                case StateUser.New:
                    if(message.ToLower() == "/start")
                    {
                        user.State = StateUser.Register0;
                        await bot.SendMessage(chatId, "Приветствую! Я ассистент для учёта продаж, пожалуйста напиши своё имя:");
                    }
                    return;
                case StateUser.Register0:
                    user.FirstName = message;
                    user.State = StateUser.Registr1;
                    await bot.SendMessage(chatId, $"Приятно познакомиться, {user.FirstName}! Теперь укажи свою роль:", replyMarkup: keyboarRoles);
                    return;
                case StateUser.Registr1:
                    if(message == "Менеджер")
                    {
                        user.Role = ChaynayRole.Manager;
                        user.State = StateUser.Registred;
                        await bot.SendMessage(chatId, "Подождите пока Администратор подтвердит вашу роль", replyMarkup: ReplyMarkup.RemoveKeyboard);

                    }
                    else if(message == "Владелец")
                    {
                        await bot.SendMessage(chatId, "Напиши пароль Администратора");
                        user.State = StateUser.ApproveAdmin;
                    }
                    else
                    {
                        await bot.SendMessage(chatId, "Пожалуйста выбери роль из предложенных вариантов:", replyMarkup: keyboarRoles);
                    }
                    return;
                case StateUser.ApproveAdmin:
                    if(message == "asde1D#cEC")
                    {
                        user.Role = ChaynayRole.Admin;
                        user.State = StateUser.Registred;
                        user.Tryes += 1;
                        await bot.SendMessage(chatId, $"Отлично! Теперь ты можешь использовать бота для учёта продаж. Выбери действие:", replyMarkup: mainKeyboardAdmin);
                    }
                    else
                    {
                        await bot.SendMessage(chatId, "Неверный пароль. Пожалуйста попробуй снова:");
                    }
                    return;
                #endregion
        
                case StateUser.Registred:
                    if(user.Role == ChaynayRole.Manager && user.IsApproved)
                    {
                        if(message == "Продажа" && TelegramBotUpdateHandler._startUp)
                        {

                            user.State = StateUser.Order0;
                        
                            var buttonRows = new List<InlineKeyboardButton[]> { };

                            Enum.GetValues<ProductChCategory>().ToList().ForEach(x =>
                            {
                                switch (x) 
                                { 
                                    case ProductChCategory.Pizza:
                                        buttonRows.Add([InlineKeyboardButton.WithCallbackData($"Пицца", $"{(int)x}")]);
                                        break;
                                    case ProductChCategory.Coffee:
                                        buttonRows.Add([InlineKeyboardButton.WithCallbackData($"Кофе", $"{(int)x}")]);
                                        break;
                                    case ProductChCategory.FruitTea:
                                        buttonRows.Add([InlineKeyboardButton.WithCallbackData($"Фруктовый чай", $"{(int)x}")]);
                                        break;
                                    case ProductChCategory.LooseLeafTea:
                                        buttonRows.Add([InlineKeyboardButton.WithCallbackData($"Рассыпной чай", $"{(int)x}")]);
                                        break;
                                    case ProductChCategory.Snacks:
                                        buttonRows.Add([InlineKeyboardButton.WithCallbackData($"Снэки", $"{(int)x}")]);
                                        break;
                                    case ProductChCategory.Desserts:
                                        buttonRows.Add([InlineKeyboardButton.WithCallbackData($"Десерты", $"{(int)x}")]);
                                        break;
                                    case ProductChCategory.Drinks:
                                        buttonRows.Add([InlineKeyboardButton.WithCallbackData($"Напитки", $"{(int)x}")]);
                                        break;
                                    case ProductChCategory.Other:
                                        buttonRows.Add([InlineKeyboardButton.WithCallbackData($"Другое", $"{(int)x}")]);
                                        break;
                                }
                            });

                            var markup = new InlineKeyboardMarkup(buttonRows);
                        
                            await bot.SendMessage(chatId, "Выберете категорию:", replyMarkup: markup);
                            return;
                        }
                        if(message == "Начать смену")
                        {
                            TelegramBotUpdateHandler._startUp = true;
                            await MessageToAdmins(bot, userService, $"Смена начата менеджером: @{user.Username}");
                            await bot.SendMessage(chatId, "Смена открыта", replyMarkup: mainKeyboardManager);

                            return;
                        }
                        if (message == "Закончить смену" && TelegramBotUpdateHandler._startUp)
                        {
                            TelegramBotUpdateHandler._startUp = false;
                            await MessageToAdmins(bot, userService, $"Смена закрыта менеджером: @{user.Username}");
                            await bot.SendMessage(chatId, "Смена закрыта", replyMarkup: mainKeyboardStart);
                            return;
                        }
                        if (!TelegramBotUpdateHandler._startUp)
                        {
                            await bot.SendMessage(chatId, "Начните смену для работы", replyMarkup: mainKeyboardStart);
                            return;
                        }
                        if (!string.IsNullOrEmpty(message))
                        {
                            await bot.SendMessage(chatId, "Кнопка не обработна", replyMarkup: TelegramBotUpdateHandler._startUp ? mainKeyboardManager : mainKeyboardStart);
                            return;
                        }
                    }
                    else if(user.Role == ChaynayRole.Admin)
                    {
                        if (message == "Работники")
                        {
                            var users = await userService.GetAllAsync();
                            var workers = users.Where(x => x.Role == ChaynayRole.Manager).ToList();
                            var buttonRows = new List<InlineKeyboardButton[]> { };

                            workers.Where(w => !w.IsApproved).ToList().ForEach(x => buttonRows.Add([InlineKeyboardButton.WithCallbackData($"{x.Username} Вкл. ✅", $"add_{x.ChatId}")]));
                            workers.Where(w => w.IsApproved).ToList().ForEach(x => buttonRows.Add([InlineKeyboardButton.WithCallbackData($"{x.Username} Искл. ⛔", $"delete_{x.ChatId}")]));

                            var markup = new InlineKeyboardMarkup(buttonRows); 

                            await bot.SendMessage(chatId, $"Всего работников: {workers.Count}", replyMarkup: markup);   
                        }
                        if (message == "Панель")
                        {
                            await bot.SendMessage(chatId, $"Панель: http://95.85.248.253/", replyMarkup: mainKeyboardAdmin);
                        }
                        if (message == "Отчёт")
                        {
                            var products = await productService.GetAllAsync();
                            var reports = await reportService.GetAllAsync();
                            var sum = 0.0m;
                            var msg = "";
                            var csvReport = "";
                            var counter = 0;
                            reports.OrderBy(x => x.CreatedAt).ToList().ForEach(rep =>
                            {
                                var product = products.FirstOrDefault(p => p.Id == rep.ProductId);
                                if (product != null)
                                {
                                    counter++;
                                    msg += $"{rep.CreatedAt:g} - {product.Name}, {rep.Count}, {product.Price:0.00}\n";
                                    csvReport += $"{rep.CreatedAt:g}, {product.Name}, {product.Price:0.00}, {rep.Count}, {rep.CreatorChatId}\n";
                                    sum += product.Price * rep.Count;
                                
                                }
                            }); 
                            msg += $"\nИтог:\n- Кол-во продаж: {counter}\n- Сумма продаж: {sum:0.00} р.";
                            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvReport)))
                            {
                                var file = new InputFileStream(stream, $"report-{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.csv");
                                await bot.SendMessage(chatId, msg);
                                await bot.SendDocument(chatId, file);
                            }
                        }
                        if (message == "/reset")
                        {
                            var resetProducts = await Products.GetAll();

                            try
                            {
                                dbContext.Database.EnsureDeleted();  // Удаляет ВСЮ базу
                                dbContext.Database.EnsureCreated(); 
                                await productService.AddProducts(resetProducts);
                                await bot.SendMessage(chatId, "База пересоздана.\n/start");
                            }
                            catch
                            {
                                throw;
                            }
                        
                        }
                    }
                return;

                case StateUser.Order02:
                    if (int.TryParse(message, out var resultData))
                    {
                        var productId = user.Age;

                        var products = await productService.GetAllAsync();

                        var currentProduct = products.FirstOrDefault(x => x.Id == productId);

                        if (currentProduct != null)
                        {
                            var createdAt = DateTime.Now;

                            var report = new Report
                            {
                                ProductId = productId,
                                Count = resultData,
                                CreatorChatId = user.ChatId,
                                CreatedAt = createdAt,
                            };

                            user.State = StateUser.Registred;

                            var markup = new InlineKeyboardButton("Отмена", $"{createdAt.ToBinary()}");

                            var users = await userService.GetAllAsync();

                            await SaveReport(user, users, bot, reportService, report, currentProduct, createdAt);
                        }
                        return;
                    }
                    else
                    {
                        await bot.SendMessage(chatId, "Вводите, только целое число!");
                        return;
                    }
            }
        }
        catch
        {
            user.State = StateUser.Registred;
            throw;
        }
    }

    public static async Task CallMessage(ITelegramBotClient bot, IUserService userService, IProductService productService, IReportService reportService, TUser user, string data, PhotoSize[]? photo = null, int? messageId = 0)
    {
        try
        {
            switch (user.Role)
        {
            case ChaynayRole.Admin:
                var dataSplit = data.Split('_');
                var action = dataSplit[0];
                var adminChatId = user.ChatId;
                if (long.TryParse(dataSplit[1], out var chatId))
                {
                    if (!string.IsNullOrEmpty(action))
                    {
                        var worker = await userService.GetUserByIdAsync(chatId);
                        
                        if (worker != null)
                        {
                            switch (action)
                            {
                                case "add":
                                    worker.IsApproved = true;
                                    await userService.UpdateUser(worker);
                                    await bot.SendMessage(chatId, "Администратор подтвердил вашу запись", replyMarkup: mainKeyboardStart);
                                    await bot.SendMessage(adminChatId, $"Работник {worker.Username} подтверждён!");

                                    break;

                                case "delete":
                                    worker.IsApproved = false;
                                    await userService.UpdateUser(worker);
                                    await bot.SendMessage(chatId, "Администратор ограничил ваш доступ", replyMarkup: ReplyMarkup.RemoveKeyboard);
                                    await bot.SendMessage(adminChatId, $"Работник {worker.Username} удалён!");
                                    
                                    break;
                            }
                            
                        }

                    }
                }

            return;
            case ChaynayRole.Manager:
                if (user.State == StateUser.Order0)
                {
                    if(int.TryParse(data, out var resultId))
                    {
                        var en = Enum.GetValues<ProductChCategory>().ToList().FirstOrDefault(x => (int)x == resultId);
                        var allProducts = await productService.GetAllAsync();
                        var prods = allProducts.Where(x => x.ProductChCategory == en).ToList();
                        var buttonRows = new List<InlineKeyboardButton[]> { };

                        prods.ForEach(x => buttonRows.Add([InlineKeyboardButton.WithCallbackData($"{x.Name} {x.Price:0.00} р.", $"{x.Id}")]));
                        
                        if(prods.Count < 1)
                        {
                            user.State = StateUser.Registred;
                            await bot.SendMessage(user.ChatId, "Позиции не добавлены в БД, обратитесь в поддержку: @roman_developer");
                        }

                        if (en == ProductChCategory.LooseLeafTea)
                            user.State = StateUser.Order01;
                        else
                            user.State = StateUser.Order1;

                        var markup = new InlineKeyboardMarkup(buttonRows);

                        if (messageId != null) await bot.EditMessageText(user.ChatId, (int)messageId, $"Позиция:", replyMarkup: markup);
                    }

                }
                else if (user.State == StateUser.Order01)
                {
                    if (int.TryParse(data, out var xId))
                    {
                        user.State = StateUser.Order02;
                        user.Age = xId;
                        await bot.SendMessage(user.ChatId, "Введите кол-во гр.\\шт.:");
                    }
                }
                else if (user.State == StateUser.Order1)
                {
                    if (int.TryParse(data, out var resultId))
                    {
                        var users = await userService.GetAllAsync();
                        var allProducts = await productService.GetAllAsync();
                        var product = allProducts.FirstOrDefault(x => x.Id == resultId);

                        if (product == null) { await bot.SendMessage(user.ChatId, $"Позиция не найдена!", replyMarkup: mainKeyboardManager); return; }
                        var createdAt = DateTime.Now;
                        var report = new Report
                        {
                            ProductId = resultId,
                            Count = 1,
                            CreatorChatId = user.ChatId,
                            CreatedAt = createdAt,
                        };

                        await SaveReport(user, users, bot, reportService, report, product, createdAt);
                    }

                }
                else if (user.State == StateUser.Registred) 
                {
                    var split = data.Split("_");
                    if (split[0] != "otmena") return;
                    if (long.TryParse(split[1], out var result))
                    {
                        
                        var bin = DateTime.FromBinary(result);
                        var report = await reportService.DeleteByTime(bin);
                        if (report != null) 
                        {
                            var users = await userService.GetAllAsync();
                            await bot.SendMessage(user.ChatId, $"Отмена");
                            var product = await productService.GetByIdAsync(report.ProductId);
                            if (product == null) return;
                            users.ToList().ForEach(x =>
                            {
                                if (x.Role == ChaynayRole.Admin)
                                {
                                    bot.SendMessage(x.ChatId, $"Менеджер @{user.Username} сделал отмену:\n{product.Name} {report.Count} (гр.\\шт.)\nCумма: {(report.Count * product.Price):0.00} руб.");
                                }
                            });
                        }
                        
                    }
                }
                return;

        }
        }
        catch
        {
            user.State = StateUser.Registred;
            throw;
        }
    }
}