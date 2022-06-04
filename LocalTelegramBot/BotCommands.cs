using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Web;
using System.Net;

using Telegram.Bot.Types.InlineQueryResults;

namespace TelegramBot.BotCommands
{
    class StartCommand : IBotCommand
    {
        public TgBot OwnerBot { get; }
        public string CommandQuery { get; } = "/start";
        public string UserFriendlyNameOfCommand { get; } = "/start";

        public string CommandDescription { get; } = "Starts interaction with bot";

        public BotCommandType CommandType { get; } = BotCommandType.SlashCommand;

        public List<BotUserRole> CommandAvailableTo { get; } = new List<BotUserRole> { BotUserRole.User, BotUserRole.Admin, BotUserRole.Moderator };

        public bool CanHaveMultipleInstances { get; } = false;

        public bool CanBeExecutedInGroupChat { get; } = true;

        public StartCommand(TgBot bot)
        {
            OwnerBot = bot;

        }


        public async Task<BotCommandProcessResult> Process(TelegramChat chat, Telegram.Bot.Types.User user, object e, object[] args)
        {
            string text = "";
            var tgChat = await OwnerBot.BotClient.GetChatAsync(chat.TgChatId);
            string guidSampleCallback = Guid.NewGuid().ToString();
            string guidDummy = Guid.NewGuid().ToString();
            var kb = new InlineKeyboardMarkup(new[]
                                      {
                                    new[]
                                    {
                                        new InlineKeyboardButton{ Text="Sample Callback command", CallbackData = guidSampleCallback}

                                    },
                                    new[]
                                    {
                                        new InlineKeyboardButton{Text="Edit Callback button", CallbackData = guidDummy}
                                    }                                  
                                });

            Message msg = null;
            if (tgChat.Type == Telegram.Bot.Types.Enums.ChatType.Private)
            {
                text = "Hi, " + user.FirstName + " this is private chat!";
                msg = await OwnerBot.BotClient.SendTextMessageAsync(chat.TgChatId, text, Telegram.Bot.Types.Enums.ParseMode.Default, replyMarkup: kb);
            }
            else
            {
                text = "Hi, " + user.FirstName + " this is group chat";
                msg = await OwnerBot.BotClient.SendTextMessageAsync(chat.TgChatId, text, Telegram.Bot.Types.Enums.ParseMode.Default, replyMarkup: kb);
            }
            string dataSampleCallback = "SampleCallbackCommand:" + /*there you can transfer anything you need in callback command*/ "e";
            OwnerBot.CallbackDataDictionary.TryAdd(guidSampleCallback, dataSampleCallback);
            string dataDummy = "EditCallbackCommand:" + /*there you can transfer anything you need in callback command*/ msg.MessageId.ToString();
            OwnerBot.CallbackDataDictionary.TryAdd(guidDummy, dataDummy);
            return BotCommandProcessResult.Succeeded;
        }

        public async Task<bool> CanBeExecuted(TelegramChat chat, Telegram.Bot.Types.User user, object e, object[] args)
        {
            return true;
        }
    }


    class InfoAndHelpCommand : IBotCommand
    {
        public TgBot OwnerBot { get; }
        public string CommandQuery { get; } = "/info";
        public string UserFriendlyNameOfCommand { get; } = "/info";
        public string CommandDescription { get; } = "In case you need help";

        public BotCommandType CommandType { get; } = BotCommandType.SlashCommand;

        public List<BotUserRole> CommandAvailableTo { get; } = new List<BotUserRole> { BotUserRole.User, BotUserRole.Admin, BotUserRole.Moderator };

        public bool CanHaveMultipleInstances { get; } = false;

        public bool CanBeExecutedInGroupChat { get; } = true;

        public InfoAndHelpCommand(TgBot bot)
        {
            OwnerBot = bot;
        }
        public async Task<bool> CanBeExecuted(TelegramChat chat, Telegram.Bot.Types.User user, object e, object[] args)
        {
            Message message = e as Message;
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return true;
        }
        public async Task<BotCommandProcessResult> Process(TelegramChat chat, Telegram.Bot.Types.User user, object e, object[] args)
        {
            Console.WriteLine("InfoAndHelp command");

            if (!await CanBeExecuted(chat, user, e, args))
            {
                return BotCommandProcessResult.CannotBeExecuted;
            }
            string text = "This is info command, you can see list of commands below "+"\n";
            foreach (var command in OwnerBot.CommonCommands)
            {
                if (!string.IsNullOrEmpty(command.UserFriendlyNameOfCommand))
                {
                    text += command.UserFriendlyNameOfCommand + " — " + command.CommandDescription + "\n";
                }
            }
            await OwnerBot.BotClient.SendTextMessageAsync(chat.TgChatId, text);
            Console.WriteLine("InfoAndHelp command started");
            await OwnerBot.BotClient.SendTextMessageAsync(chat.TgChatId, "sample  contact message (why not?)");
            await OwnerBot.BotClient.SendContactAsync(
                 chatId: chat.TgChatId,
                 phoneNumber: "+123456789",
                 firstName: "Telegram Bot",
                 vCard: "BEGIN:VCARD\n" +
             "VERSION:3.0\n" +
             "N:Telegram;Bot\n" +
             "ORG:Developer\n" +
             "TEL;TYPE=voice,work,pref:+79630497151\n" +
             "EMAIL:ayy@lmao.com\n" +
             "END:VCARD");          
            return BotCommandProcessResult.Succeeded;
        }
    }
}
