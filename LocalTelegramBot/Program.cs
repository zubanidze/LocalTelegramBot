using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.InlineQueryResults;
using System.Threading;
using System.Windows.Forms;

namespace TelegramBot
{

    class Program
    {
        static ITelegramBotClient botClient;

        static void Main(string[] args)
        {
            string tokenValue = ""; //put your token here
            botClient = new TelegramBotClient(tokenValue);
            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
              $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );

            TgBot tgBot = new TgBot(botClient);
            ThreadStart botThreadStart = new ThreadStart(tgBot.Start);
            Thread botThread = new Thread(botThreadStart);
            botThread.Priority = ThreadPriority.AboveNormal;
            botThread.Start();

            while (true)
            {
                if (Console.ReadLine() == "status")
                {
                    Console.WriteLine(tgBot.BotClient.IsReceiving.ToString());

                }

            }

            tgBot.BotClient.StopReceiving();
        }
    }
}
