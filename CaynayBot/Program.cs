using CaynayBot;
using Microsoft.Extensions.Logging;

class Program
{
    public static void Main(string[] args)
    {
        var logger = new TLogger();
        var bot = new TelegramBotUpdateHandler("5589735895:AAESnngDwmobK5wlviYb53F3yIQBVUr9iK4", logger);
        bot.Start();

        while (true) 
        {
            Console.ReadLine();
        }
    }
}