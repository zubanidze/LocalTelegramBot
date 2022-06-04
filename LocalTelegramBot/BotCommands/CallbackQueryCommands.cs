using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.CallbackQueryCommands
{
    class SampleCallBackCommand : IBotCommand
    {
        public TgBot OwnerBot { get; }
        public string CommandQuery { get; } = "SampleCallbackCommand";
        public string UserFriendlyNameOfCommand { get; }
        public string CommandDescription { get; }

        public BotCommandType CommandType { get; } = BotCommandType.CallbackCommand;

        public List<BotUserRole> CommandAvailableTo { get; } = new List<BotUserRole> { BotUserRole.User, BotUserRole.Admin, BotUserRole.Moderator };

        public bool CanHaveMultipleInstances { get; }

        public bool CanBeExecutedInGroupChat { get; } = false;

        public SampleCallBackCommand(TgBot bot)
        {
            OwnerBot = bot;
        }

        public async Task<bool> CanBeExecuted(TelegramChat chat, Telegram.Bot.Types.User user, object e, object[] args)
        {
            return true;
        }

        public async Task<BotCommandProcessResult> Process(TelegramChat chat, Telegram.Bot.Types.User user, object e, object[] args)
        {

            CallbackQuery query = e as CallbackQuery;
            await OwnerBot.BotClient.AnswerCallbackQueryAsync(query.Id, "Congrats you clicked this button!", false);
            await OwnerBot.BotClient.SendTextMessageAsync(query.Message.Chat.Id, text: "This message was triggered by inline keyboard button");
            return BotCommandProcessResult.Succeeded;
        }
    }
    class EditCallBackCommand : IBotCommand
    {
        public TgBot OwnerBot { get; }
        public string CommandQuery { get; } = "EditCallbackCommand";
        public string UserFriendlyNameOfCommand { get; }
        public string CommandDescription { get; }

        public BotCommandType CommandType { get; } = BotCommandType.CallbackCommand;

        public List<BotUserRole> CommandAvailableTo { get; } = new List<BotUserRole> { BotUserRole.User, BotUserRole.Admin, BotUserRole.Moderator };

        public bool CanHaveMultipleInstances { get; }

        public bool CanBeExecutedInGroupChat { get; } = false;

        public EditCallBackCommand(TgBot bot)
        {
            OwnerBot = bot;
        }

        public async Task<bool> CanBeExecuted(TelegramChat chat, Telegram.Bot.Types.User user, object e, object[] args)
        {
            return true;
        }

        public async Task<BotCommandProcessResult> Process(TelegramChat chat, Telegram.Bot.Types.User user, object e, object[] args)
        {

            CallbackQuery query = e as CallbackQuery;
            await OwnerBot.BotClient.AnswerCallbackQueryAsync(query.Id, "Congrats you decided to edit this msg this button!", false);
            await OwnerBot.BotClient.EditMessageTextAsync(query.Message.Chat.Id, query.Message.MessageId, text: "This message was edited by inline keyboard button");
            return BotCommandProcessResult.Succeeded;
        }
    }
}
